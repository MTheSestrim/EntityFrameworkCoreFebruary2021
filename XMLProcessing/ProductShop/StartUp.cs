namespace ProductShop
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using ProductShop.Data;
    using ProductShop.Dtos.Export;
    using ProductShop.Dtos.Import;
    using ProductShop.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class StartUp
    {
        private static IMapper mapper;
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //string usersXML = File.ReadAllText("../../../Datasets/users.xml");
            //string productsXML = File.ReadAllText("../../../Datasets/products.xml");
            //string categoriesXML = File.ReadAllText("../../../Datasets/categories.xml");
            //string categoriesProductsXML = File.ReadAllText("../../../Datasets/categories-products.xml");

            //ImportUsers(context, usersXML);
            //ImportProducts(context, productsXML);
            //ImportCategories(context, categoriesXML);
            //ImportCategoryProducts(context, categoriesProductsXML);

            Console.WriteLine(GetUsersWithProducts(context));
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            InitializeAutoMapper();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserInputModel[]), new XmlRootAttribute("Users"));
            var textReader = new StringReader(inputXml);

            var dtoUsers = xmlSerializer.Deserialize(textReader) as UserInputModel[];

            var users = mapper.Map<IEnumerable<User>>(dtoUsers);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            InitializeAutoMapper();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProductInputModel[]), 
                new XmlRootAttribute("Products"));
            var textReader = new StringReader(inputXml);

            var dtoProducts = xmlSerializer.Deserialize(textReader) as IEnumerable<ProductInputModel>;

            var products = mapper.Map<IEnumerable<Product>>(dtoProducts);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            InitializeAutoMapper();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CategoryInputModel[]),
                new XmlRootAttribute("Categories"));
            var textReader = new StringReader(inputXml);

            var dtoCategories = xmlSerializer.Deserialize(textReader) as IEnumerable<CategoryInputModel>;

            var categories = mapper.Map<IEnumerable<Category>>(dtoCategories).Where(c => c.Name != null);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            InitializeAutoMapper();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CategoryProductInputModel[]),
                new XmlRootAttribute("CategoryProducts"));
            var textReader = new StringReader(inputXml);

            var dtoCategoryProducts = xmlSerializer
                .Deserialize(textReader) as IEnumerable<CategoryProductInputModel>;

            var categoryProducts = mapper
                .Map<IEnumerable<CategoryProduct>>(dtoCategoryProducts);

            var existingCategoryIds = context
                .Categories
                .Select(c => c.Id)
                .ToList();

            var existingProductIds = context
                .Products
                .Select(c => c.Id)
                .ToList();

            //The ids could be extracted before categoryProducts is even mapped.
            //Afterwards, the "Where" method could be used directly on the same enumeration without
            //the need of an extra variable, similar to how I've done it in the categories import.
            //However, I've separated the workflow in this manner so as to make the code easier to read.
            var categoryProductsTrimmed = categoryProducts
                    .Where(cp =>
                        existingCategoryIds.Any(ec => ec == cp.CategoryId)
                        &&
                        existingProductIds.Any(ep => ep == cp.ProductId));

            context
                .CategoryProducts
                .AddRange(categoryProductsTrimmed);
            context.SaveChanges();

            return $"Successfully imported {categoryProductsTrimmed.Count()}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            InitializeAutoMapper();
            var dtoProducts = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .ProjectTo<ProductInRangeExportModel>(mapper.ConfigurationProvider)
                .ToArray();

            StringBuilder result = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProductInRangeExportModel[]),
                new XmlRootAttribute("Products"));

            xmlSerializer.Serialize(new StringWriter(result), dtoProducts, namespaces);

            return result.ToString().Trim();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            InitializeAutoMapper();

            var dtoUsers = context
                .Users
                .Where(u => u.ProductsSold.Count > 0)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ProjectTo<UserSoldProductExportModel>(mapper.ConfigurationProvider)
                .ToArray();

            StringBuilder result = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            var xmlSerializer = new XmlSerializer(typeof(UserSoldProductExportModel[]), new XmlRootAttribute("Users"));

            xmlSerializer.Serialize(new StringWriter(result), dtoUsers, namespaces);

            return result.ToString().Trim();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            InitializeAutoMapper();

            var dtoCategories = context
                .Categories
                .ProjectTo<CategoryExportModel>(mapper.ConfigurationProvider)
                .OrderByDescending(ce => ce.Count)
                .ThenBy(ce => ce.TotalRevenue)
                .ToArray();

            StringBuilder result = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            var xmlSerializer = new XmlSerializer(
                typeof(CategoryExportModel[]), 
                new XmlRootAttribute("Categories"
                ));

            xmlSerializer.Serialize(new StringWriter(result), dtoCategories, namespaces);

            return result.ToString().Trim();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            InitializeAutoMapper();

            var dtoUsers = context
                .Users
                .Where(u => u.ProductsSold.Count > 0)
                //It works without converting it to an array. However, Judge won't accept it.
                .ToArray()
                .Select(u => new UserAndProductUserExportModel
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new  UserAndProductSoldProductExportModel
                    { 
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold.Select(p => new UserAndProductProductExportModel 
                        {
                            Name = p.Name,
                            Price = p.Price
                        }).OrderByDescending(p => p.Price).ToArray(),
                    },
                })
                .OrderByDescending(uapu => uapu.SoldProducts.Count)
                .Take(10)
                .ToArray();

            var finalModel = new UserAndProductFinalExportModel
            {
                Count = context.Users.Where(u => u.ProductsSold.Count > 0).Count(),
                Users = dtoUsers
            };

            StringBuilder result = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            var xmlSerializer = new XmlSerializer(
                typeof(UserAndProductFinalExportModel),
                new XmlRootAttribute("Users"
                ));

            xmlSerializer.Serialize(new StringWriter(result), finalModel, namespaces);

            return result.ToString().Trim();
        }

        private static void InitializeAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });
            mapper = config.CreateMapper();
        }
    }
}