using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using myshop.Business.DTOs;
using myshop.Business.Extentions;
using myshop.Business.Services.Interfaces;
using System;
using System.IO;

namespace myshop.Business.Services.Implementations 
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKey = "ShoppingCartSession";

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public Cart GetCart()
        {
            var cart = Session.GetObjectFromJson<Cart>(CartSessionKey);
            if (cart == null)
            {
                cart = new Cart();
                Session.SetObjectAsJson(CartSessionKey, cart);
            }
            return cart;
        }

        public void AddToCart(CartItem item)
        {
            var cart = GetCart();
            var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == item.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Items.Add(item);
            }

            Session.SetObjectAsJson(CartSessionKey, cart);
        }

        public void RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                cart.Items.Remove(item);
                Session.SetObjectAsJson(CartSessionKey, cart);
            }
        }

        public void IncreaseQuantity(int productId)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                item.Quantity++;
                Session.SetObjectAsJson(CartSessionKey, cart);
            }
        }

        public void DecreaseQuantity(int productId)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                item.Quantity--;
                if (item.Quantity <= 0)
                {
                    cart.Items.Remove(item);
                }
                Session.SetObjectAsJson(CartSessionKey, cart);
            }
        }

        public void ClearCart()
        {
            Session.Remove(CartSessionKey);
        }
    }
}