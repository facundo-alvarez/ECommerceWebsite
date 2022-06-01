﻿using ApplicationCore.Entities;
using ApplicationCore.ValueObjects;

namespace ApplicationCore.Interfaces
{
    public interface IOrderProductService
    {
        IEnumerable<Order_Product> GetOrderCurrentProducts(int orderId);
        void RemoveProductFormCurrentOrder(int orderId, int productId);
        void SetOrderProducts(List<Item> items, int orderId);
        void AddProductQuantity(int orderId, int productId, int quantity);
        void RemoveProductQuantity(int orderId, int productId, int quantity);
        decimal GetOrderSubtotal(int orderId);
    }
}
