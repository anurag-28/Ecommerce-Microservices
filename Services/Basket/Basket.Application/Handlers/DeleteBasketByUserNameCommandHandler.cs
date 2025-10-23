using Basket.Application.Commands;
using Basket.Core.Repositories;
using MediatR;

namespace Basket.Application.Handlers
{
    public class DeleteBasketByUserNameCommandHandler : IRequestHandler<DeleteBasketByUserNameCommand, bool>
    {
        private readonly IBasketRepository _basketRepository;
        public DeleteBasketByUserNameCommandHandler(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }
        public async Task<bool> Handle(DeleteBasketByUserNameCommand request, CancellationToken cancellationToken)
        {
            var deleted = await _basketRepository.DeleteBasket(request.UserName);
            return deleted;
        }
    }
}
