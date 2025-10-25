using Grpc.Core;
using Discount.Application.Queries;
using Discount.Core.Repositories;
using Discount.Grpc.Protos;
using MediatR;

namespace Discount.Application.Handlers
{
    public class GetDiscountQueryHandler : IRequestHandler<GetDiscountQuery, CouponModel>
    {
        private readonly IDiscountRepository _discountRepository;
        public GetDiscountQueryHandler(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }
        public async Task<CouponModel> Handle(GetDiscountQuery request, CancellationToken cancellationToken)
        {
            var coupon = await _discountRepository.GetDiscount(request.Productname);
            if(coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.Productname} is not found."));
            }
            var couponModel = new CouponModel
                {
                Id = coupon.Id,
                ProductName = coupon.ProductName,
                Description = coupon.Description,
                Amount = coupon.Amount
            };
            return couponModel;
        }
    }
}
