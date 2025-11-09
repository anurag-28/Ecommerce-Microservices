using Discount.Application.Queries;
using Discount.Application.Commands;
using Discount.Grpc.Protos;
using Grpc.Core;
using MediatR;
namespace Discount.API.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DiscountService> _logger;
        public DiscountService(IMediator mediator, ILogger<DiscountService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Fetching discount for Product: {ProductName}", request.ProductName);

            var query = new GetDiscountQuery(request.ProductName);
            var result = await _mediator.Send(query);

            if (result == null)
                _logger.LogWarning("No discount found for Product: {ProductName}", request.ProductName);
            else
                _logger.LogInformation("Successfully fetched discount for Product: {ProductName}", request.ProductName);

            return result;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Creating discount for Product: {ProductName}", request.Coupon.ProductName);

            var command = new CreateDiscountCommand
            {
                ProductName = request.Coupon.ProductName,
                Description = request.Coupon.Description,
                Amount = request.Coupon.Amount
            };

            var result = await _mediator.Send(command);
            _logger.LogInformation("Successfully created discount for Product: {ProductName}", result.ProductName);

            return result;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Updating discount for Product: {ProductName}", request.Coupon.ProductName);

            var command = new UpdateDiscountCommand
            {
                Id = request.Coupon.Id,
                ProductName = request.Coupon.ProductName,
                Description = request.Coupon.Description,
                Amount = request.Coupon.Amount
            };

            var result = await _mediator.Send(command);
            _logger.LogInformation("Successfully updated discount for Product: {ProductName}", result.ProductName);

            return result;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Deleting discount for Product: {ProductName}", request.ProductName);

            var command = new DeleteDiscountCommand(request.ProductName);
            var isDeleted = await _mediator.Send(command);
            if (isDeleted)
                _logger.LogInformation("Successfully deleted discount for Product: {ProductName}", request.ProductName);
            else
                _logger.LogWarning("No discount found to delete for Product: {ProductName}", request.ProductName);
            var response = new DeleteDiscountResponse
            {
                Success = isDeleted
            };
            return response;
        }
    }
}
