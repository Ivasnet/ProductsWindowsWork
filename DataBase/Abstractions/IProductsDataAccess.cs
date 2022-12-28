using DataBase.Models;
using System.Collections.Generic;

namespace DataBase.Abstractions
{
    public interface IProductsDataAccess
    {
        void CreateIfNotExistsDataBase();

        void AddProduct(Product product);

        ProductCounter AddOrIncrementReceptionProductCounter(string id);

        ProductCounter AddOrIncrementShipmentProductCounter(string id);

        void AddGroupProducts(IEnumerable<Product> products);

        void RemoveAllProducts();

        void RemoveAllProductsCounters();

        void RemoveProduct(string id);

        void RemoveReceptionProductCounter(string id);

        void RemoveShipmentProductCounter(string id);

        IEnumerable<Product> GetAllProducts();

        IEnumerable<ProductCounter> GetAllReceptionProducts();

        IEnumerable<ProductCounter> GetAllShipmentProducts();

        void DeleteDataBase();
    }
}
