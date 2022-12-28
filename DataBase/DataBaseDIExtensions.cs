using DataBase.Abstractions;
using DataBase.Core;
using Microsoft.Extensions.DependencyInjection;

namespace DataBase
{
    public static class DataBaseDIExtensions
    {
        public static IServiceCollection AddDbServices(this IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddSingleton<IProductsDataAccess, ProductsDataAccess>();

            return serviceDescriptors;
        }
    }
}
