using Asp.Versioning;
using Basket.Application.Commands;
using Basket.Application.GrpcService;
using Basket.Application.Mappers;
using Basket.Application.Queries;
using Basket.Application.Responses;
using Basket.Core.Entities;
using EventBus.Messages.Common;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.API.Controllers
{
    [ApiVersion("1")]
    public class BasketController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<BasketController> _logger;
        public BasketController(IMediator mediator, IPublishEndpoint publishEndpoint, ILogger<BasketController> logger)
        {
            _mediator = mediator;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]/{userName}", Name = "GetBasketByUserName")]
        [ProducesResponseType(typeof(ShoppingCartResponse), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCartResponse>> GetBasket(string userName)
        {
            _logger.LogInformation("Fetching basket for user: {UserName}", userName);
            var query = new GetBasketByUserNameQuery(userName);
            var basket = await _mediator.Send(query);
            if(basket == null)
            {
                _logger.LogWarning("No basket found for user: {UserName}", userName);
                return NotFound();
            }
            _logger.LogInformation("Basket retrieved successfully for user: {UserName}", userName);
            return Ok(basket);
        }

        [HttpPost("CreateBasket")]
        [ProducesResponseType(typeof(ShoppingCartResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCartResponse>> UpdateBasket([FromBody] CreateShoppingCartCommand createShoppingCartCommand)
        {
            _logger.LogInformation("Creating or updating basket for user: {UserName}", createShoppingCartCommand.UserName);
            var basket = await _mediator.Send(createShoppingCartCommand);
            _logger.LogInformation("Basket successfully created/updated for user: {UserName} with {ItemCount} items",
                createShoppingCartCommand.UserName, basket?.Items?.Count ?? 0);
            return Ok(basket);
        }

        [HttpDelete]
        [Route("[action]/{userName}", Name = "DeleteBasketByUserName")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteBasket(string userName)
        {
            _logger.LogInformation("Deleting basket for user: {UserName}", userName);

            var command = new DeleteBasketByUserNameCommand(userName);
            var isDeleted = await _mediator.Send(command);

            if (isDeleted)
                _logger.LogInformation("Basket deleted successfully for user: {UserName}", userName);
            else
                _logger.LogWarning("No basket found to delete for user: {UserName}", userName);

            return Ok(isDeleted);
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            //Get the existing basket with username
            _logger.LogInformation("Initiating checkout for user: {UserName}", basketCheckout.UserName);

            var query = new GetBasketByUserNameQuery(basketCheckout.UserName);
            var basket = await _mediator.Send(query);
            if (basket == null)
            {
                _logger.LogWarning("Checkout failed: No basket found for user: {UserName}", basketCheckout.UserName);
                return BadRequest();
            }

            var eventMsg = BasketMapper.Mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMsg.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMsg);
            _logger.LogInformation($"Basket Published for {basket.UserName}");
            //remove the basket
            var deleteCmd = new DeleteBasketByUserNameCommand(basketCheckout.UserName);
            await _mediator.Send(deleteCmd);
            _logger.LogInformation("Basket deleted after successful checkout for user: {UserName}", basketCheckout.UserName);
            return Accepted();
        }
    }
}
