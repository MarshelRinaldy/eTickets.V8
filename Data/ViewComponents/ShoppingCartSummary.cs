using eTickets.V8.Data.Cart;
using Microsoft.AspNetCore.Mvc;

namespace eTickets.V8.Data.ViewComponents
{
    public class ShoppingCartSummary:ViewComponent
    {
        //ini untuk mendapatkan jumlah dari item pada cart
        private readonly ShoppingCart _shoppingCart;

        public ShoppingCartSummary(ShoppingCart shoppingCart)
        {
            _shoppingCart = shoppingCart;
        }

        public IViewComponentResult Invoke()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            return View(items.Count);
        }
    }
}
