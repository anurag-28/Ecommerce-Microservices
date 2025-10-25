
using Discount.Grpc.Protos;
using MediatR;

namespace Discount.Application.Queries
{
    public class GetDiscountQuery : IRequest<CouponModel>
    {
        public string Productname { get; set; }
        public GetDiscountQuery(string productName)
        {
            Productname = productName;
        }
    }
}
