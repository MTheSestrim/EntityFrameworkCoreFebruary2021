namespace ProductShop
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using ProductShop.Data;
    using ProductShop.DataTransferObjects.ExportDTO.Task5DTO;
    using ProductShop.DataTransferObjects.ExportDTO.Task7DTO;
    using ProductShop.DataTransferObjects.ImportDTO.Task1DTO;
    using ProductShop.DataTransferObjects.ImportDTO.Task2DTO;
    using ProductShop.DataTransferObjects.ImportDTO.Task3DTO;
    using ProductShop.DataTransferObjects.ImportDTO.Task4DTO;
    using ProductShop.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    public class StartUp
    {
        static IMapper mapper;
        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //string usersInputJson = File.ReadAllText("../../../Datasets/users.json");
            //string productsInputJson = File.ReadAllText("../../../Datasets/products.json");
            //string categoriesInputJson = File.ReadAllText("../../../Datasets/categories.json");
            //string categoriesProductsInputJson = File.ReadAllText("../../../Datasets/categories-products.json");
            //ImportUsers(context, usersInputJson);
            //ImportProducts(context, productsInputJson);
            //ImportCategories(context, categoriesInputJson);
            //Console.WriteLine(ImportCategoryProducts(context, categoriesProductsInputJson));

            Console.WriteLine(GetUsersWithProducts(context));
        }

        private static void InitializeAutomapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            mapper = config.CreateMapper();
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            InitializeAutomapper();

            var dtoUsers = JsonConvert.DeserializeObject<IEnumerable<UserInputModel>>(inputJson);

            var users = mapper.Map<IEnumerable<User>>(dtoUsers);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            InitializeAutomapper();

            var dtoProducts = JsonConvert.DeserializeObject<IEnumerable<ProductInputModel>>(inputJson);

            var products = mapper.Map<IEnumerable<Product>>(dtoProducts);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            InitializeAutomapper();

            //var deserializerSettings = new JsonSerializerSettings() 
            //{ NullValueHandling = NullValueHandling.Ignore };

            var dtoCategories = JsonConvert
                .DeserializeObject<IEnumerable<CategoryInputModel>>(inputJson)
                .Where(c => c.Name != null);

            var categories = mapper.Map<IEnumerable<Category>>(dtoCategories);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            InitializeAutomapper();

            var dtoCategProds = JsonConvert.DeserializeObject<IEnumerable<CategoryProductInputModel>>(inputJson);

            var categProds = mapper.Map<IEnumerable<CategoryProduct>>(dtoCategProds);

            context.CategoryProducts.AddRange(categProds);
            context.SaveChanges();

            return $"Successfully imported {categProds.Count()}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            InitializeAutomapper();

            var products = context
                .Products
                .ProjectTo<ProductExportOutputModel>(mapper.ConfigurationProvider)
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .ToList();

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //settings.Formatting = Formatting.Indented;

            string result = JsonConvert.SerializeObject(products, settings);

            return result;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var sellers = context
                .Users
                .Select(s => new
                {
                    s.FirstName,
                    s.LastName,
                    SoldProducts = s.ProductsSold.Select(p => new { p.Name, p.Price, BuyerFirstName = p.Buyer.FirstName, BuyerLastName = p.Buyer.LastName }).Where(p => p.BuyerLastName != null)
                })
                .Where(s => s.SoldProducts.Count() >= 1)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToList();

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            string result = JsonConvert.SerializeObject(sellers, settings);

            return result;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            InitializeAutomapper();

            var categories = context
                .Categories
                .ProjectTo<CategoryByProductCountOutputModel>(mapper.ConfigurationProvider)
                .OrderByDescending(c => c.ProductsCount)
                .ToList();

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            string result = JsonConvert.SerializeObject(categories, settings);

            return result;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Include(x => x.ProductsSold)
                .ToList()
                .Where(u => u.ProductsSold.Any(b => b.BuyerId != null))
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.Age,
                    SoldProducts = new
                    {
                        Count = u.ProductsSold.Where(x => x.BuyerId != null).Count(),
                        Products = u.ProductsSold.Where(x => x.BuyerId != null).Select(p => new
                        {
                            p.Name,
                            p.Price
                        })
                    }
                })
                .OrderByDescending(x => x.SoldProducts.Count);

            var resultObject = new
            {
                UsersCount = users.Count(),
                users
            };

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            settings.Formatting = Formatting.Indented;

            string result = JsonConvert.SerializeObject(resultObject, settings);

            return result;
        }
    }
}