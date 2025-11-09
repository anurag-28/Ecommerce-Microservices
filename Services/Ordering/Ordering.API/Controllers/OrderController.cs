using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Commands;
using Ordering.Application.Queries;
using Ordering.Application.Responses;
using System.Net;

namespace Ordering.API.Controllers
{
    public class OrderController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IMediator mediator, ILogger<OrderController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{userName}", Name = "GetOrdersByUserName")]
        [ProducesResponseType(typeof(IEnumerable<OrderResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrdersByUserName(string userName)
        {
            _logger.LogInformation("Fetching orders for user: {UserName}", userName);
            var query = new GetOrderListQuery(userName);
            var orders = await _mediator.Send(query);
            if (orders == null || !orders.Any())
                _logger.LogWarning("No orders found for user: {UserName}", userName);
            else
                _logger.LogInformation("Fetched {Count} orders for user: {UserName}", orders.Count(), userName);

            return Ok(orders);
        }
        //Just for testing 
        [HttpPost(Name = "CheckoutOrder")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> CheckoutOrder([FromBody] CheckoutOrderCommand command)
        {
            _logger.LogInformation("Processing checkout order for user: {UserName}", command.UserName);

            var result = await _mediator.Send(command);

            _logger.LogInformation("Checkout order completed successfully for user: {UserName} with Order ID: {OrderId}",
                command.UserName, result);

            return Ok(result);
        }
        [HttpPut(Name = "UpdateOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> UpdateOrder([FromBody] UpdateOrderCommand command)
        {
            _logger.LogInformation("Updating order with ID: {OrderId}", command.Id);

            var result = await _mediator.Send(command);

            _logger.LogInformation("Successfully updated order with ID: {OrderId}", command.Id);

            return NoContent();
        }
        [HttpDelete("{id}", Name = "DeleteOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<bool> DeleteOrder(int id)
        {
            _logger.LogInformation("Deleting order with ID: {OrderId}", id);

            var cmd = new DeleteOrderCommand { Id = id };
            await _mediator.Send(cmd);

            _logger.LogInformation("Successfully deleted order with ID: {OrderId}", id);

            return true;
        }
    }
}