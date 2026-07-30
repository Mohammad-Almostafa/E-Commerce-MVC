using Microsoft.AspNetCore.Http;
using myshop.Business.DTOs;

namespace myshop.Business.Services.Interfaces
{
    public interface ICartService
    {
        Cart GetCart();
        void AddToCart(CartItem item);
        void RemoveFromCart(int productId);
        void IncreaseQuantity(int productId);
        void DecreaseQuantity(int productId);
        void ClearCart();
    }
}