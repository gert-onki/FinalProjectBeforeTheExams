using shittyEtsy.Data.Models;
using System.Collections.Generic;

namespace shittyEtsy.Session
{
    public static class CartManager
    {
        private static readonly List<Products> _items = new();

        public static IReadOnlyList<Products> Items => _items.AsReadOnly();

        public static int Count => _items.Count;

        public static bool Add(Products product)
        {
            if (_items.Exists(p => p.Id == product.Id))
                return false; // already in cart

            _items.Add(product);
            return true;
        }

        public static void Remove(int productId)
        {
            _items.RemoveAll(p => p.Id == productId);
        }

        public static bool Contains(int productId)
        {
            return _items.Exists(p => p.Id == productId);
        }

        public static void Clear()
        {
            _items.Clear();
        }

        public static decimal Total()
        {
            decimal sum = 0;
            foreach (var p in _items)
                sum += p.Price;
            return sum;
        }
    }
}
