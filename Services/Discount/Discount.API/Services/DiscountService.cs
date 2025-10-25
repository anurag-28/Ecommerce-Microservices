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
        public DiscountService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var query = new GetDiscountQuery(request.ProductName);
            var result = await _mediator.Send(query);
            return result;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var command = new CreateDiscountCommand
            {
                ProductName = request.Coupon.ProductName,
                Description = request.Coupon.Description,
                Amount = request.Coupon.Amount
            };
            var result =  await _mediator.Send(command);
            return result;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var command = new UpdateDiscountCommand
            {
                Id = request.Coupon.Id,
                ProductName = request.Coupon.ProductName,
                Description = request.Coupon.Description,
                Amount = request.Coupon.Amount
            };
            var result = await _mediator.Send(command);
            return result;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var command = new DeleteDiscountCommand(request.ProductName);
            var isDeleted = await _mediator.Send(command);
            var response = new DeleteDiscountResponse
            {
                Success = isDeleted
            };
            return response;
        }
    }
}
