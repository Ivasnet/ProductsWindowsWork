using DataBase.Abstractions;
using DataBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataBase.Core
{
    public class ProductsDataAccess : IProductsDataAccess
    {
        public void CreateIfNotExistsDataBase()
        {
            using (var context = new ProductContext())
            {
                context.Database.CreateIfNotExists();
            }
        }

        public void DeleteDataBase()
        {
            using (var context = new ProductContext())
            {
                context.Database.Delete();
            }
        }

        public void AddProduct(Product product)
        {
            using (var context = new ProductContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (!context.Products.Any(x => x.Id == product.Id))
                        {
                            context.Products.Add(product);

                            context.SaveChanges();

                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        //logging
                        transaction.Rollback();

                        throw;
                    }
                }
            }
        }

        public ProductCounter AddOrIncrementReceptionProductCounter(string id) => AddOrUpdateProductCounter(id,
                productCounter =>
                {
                    if (productCounter.Shipment > 0)
                    {
                        productCounter.Shipment--;
                    }
                },
                productCounter => productCounter.Reception++,
                (product, context) => {
                    var productCounter = new ProductCounter()
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Reception = 1
                    };
                    context.ProductsCounters
                            .Add(productCounter);
                    return productCounter;
                },

                RemoveShipmentProductCounter);

        public ProductCounter AddOrIncrementShipmentProductCounter(string id) => AddOrUpdateProductCounter(id,
                productCounter => 
                {
                    if (productCounter.Reception > 0)
                    {
                        productCounter.Reception--;
                    }
                },
                productCounter => productCounter.Shipment++,
                (product, context) => {
                    var productCounter = new ProductCounter()
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Shipment = 1
                    };
                    context.ProductsCounters
                            .Add(productCounter);
                    return productCounter;
                },

                RemoveReceptionProductCounter);

        private ProductCounter AddOrUpdateProductCounter(string id,
            Action<ProductCounter> DecrimentIfMoreZero,
            Action<ProductCounter> updateAction, 
            Func<Product, ProductContext, ProductCounter> addAction, 
            Action<string> removeAction)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            try
            {
                using (var context = new ProductContext())
                {
                    var product = context.Products.FirstOrDefault(x => x.Id == id);

                    if (product != null)
                    {
                        var productCounter = context.ProductsCounters.FirstOrDefault(x => x.Id == id);

                        if (productCounter == null)
                        {
                            productCounter = addAction(product, context);
                        }

                        else
                        {
                            removeAction(id);

                            DecrimentIfMoreZero(productCounter);

                            updateAction(productCounter);
                        }

                        context.SaveChanges();

                        return context.ProductsCounters.FirstOrDefault(x => x.Id == id);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                //logging

                throw;
            }
        }

        public void AddGroupProducts(IEnumerable<Product> entities)
        {
            using (var context = new ProductContext())
            {                
                try
                {
                    foreach (var entity in entities)
                    {
                        if (!context.Products.Any(x => x.Id == entity.Id))
                        {
                            context.Products.Add(entity);
                        }
                    }                        

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    //logging

                    throw;
                }
            }
        }

        public IEnumerable<Product> GetAllProducts()
        {
            using (var context = new ProductContext())
            {
                return context.Products;
            }
        }

        public IEnumerable<ProductCounter> GetAllReceptionProducts()
        {
            try
            {
                using (var context = new ProductContext())
                {
                    return context.ProductsCounters.Where(x => x.Reception > 0).ToList();
                }
            }
            catch (Exception ex)
            {
                //logging

                throw;
            }
        }

        public IEnumerable<ProductCounter> GetAllShipmentProducts()
        {
            try
            {
                using (var context = new ProductContext())
                {
                    return context.ProductsCounters.Where(x => x.Shipment > 0).ToList();
                }
            }
            catch (Exception ex)
            {
                //logging

                throw;
            }
        }

        public void RemoveAllProducts()
        {
            try
            {
                using (var context = new ProductContext())
                {
                    foreach (var entity in context.Products)
                    {
                        context.Products.Remove(entity);
                    }

                    foreach (var entity in context.ProductsCounters)
                    {
                        context.ProductsCounters.Remove(entity);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                //logging

                throw;
            }
        }

        public void RemoveAllProductsCounters()
        {
            try
            {
                using (var context = new ProductContext())
                {
                    foreach (var entity in context.ProductsCounters)
                    {
                        context.ProductsCounters.Remove(entity);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                //logging

                throw;
            }
        }

        public void RemoveProduct(string id)
        {
            using (var context = new ProductContext())
            {                
                try
                {
                    if (context.Products.Any(x => x.Id == id))
                    {
                        context.Products.Remove(context.Products.Find(id));

                        if (context.ProductsCounters.Any(x => x.Id == id))
                        {
                            context.ProductsCounters.Remove(context.ProductsCounters.Find(id));
                        }                            

                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    //logging

                    throw;
                }                
            }
        }

        public void RemoveReceptionProductCounter(string id)
        {
            using (var context = new ProductContext())
            {                
                try
                {
                    var product = context.ProductsCounters.FirstOrDefault(x => x.Id == id && x.Reception > 0);

                    if (product != null)
                    {
                        product.Reception--;

                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    //logging

                    throw;
                }
            }
        }

        public void RemoveShipmentProductCounter(string id)
        {
            using (var context = new ProductContext())
            {                
                try
                {
                    var product = context.ProductsCounters.FirstOrDefault(x => x.Id == id && x.Shipment > 0);

                    if (product != null)
                    {
                        product.Shipment--;

                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    //logging

                    throw;
                }
            }
        }
    }
}
