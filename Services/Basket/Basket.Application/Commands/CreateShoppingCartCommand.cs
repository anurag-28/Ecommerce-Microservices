using MediatR;
using Basket.Application.Responses;
using Basket.Core.Entities;

namespace Basket.Application.Commands
{
    public class CreateShoppingCartCommand : IRequest<ShoppingCartResponse>
    {
        public string UserName { get; set; }
        public List<ShoppingCartItem> Items { get; set; }

        public CreateShoppingCartCommand(string username, List<ShoppingCartItem> items) 
        {
            UserName = username;
            Items = items;
        }
    }
}
