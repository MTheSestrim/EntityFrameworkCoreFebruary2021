namespace ProductShop
{
    using AutoMapper;
    using ProductShop.DataTransferObjects.ExportDTO.Task5DTO;
    using ProductShop.DataTransferObjects.ExportDTO.Task7DTO;
    using ProductShop.DataTransferObjects.ImportDTO.Task1DTO;
    using ProductShop.DataTransferObjects.ImportDTO.Task2DTO;
    using ProductShop.DataTransferObjects.ImportDTO.Task3DTO;
    using ProductShop.DataTransferObjects.ImportDTO.Task4DTO;
    using ProductShop.Models;
    using System.Linq;
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            //Task 1
            this.CreateMap<UserInputModel, User>();
            //Task 2
            this.CreateMap<ProductInputModel, Product>();
            //Task 3
            this.CreateMap<CategoryInputModel, Category>();
            //Task 4
            this.CreateMap<CategoryProductInputModel, CategoryProduct>();
            //Task 5
            this.CreateMap<Product, ProductExportOutputModel>()
                .ForMember(pe => pe.Seller, 
                p => p.MapFrom(s => 
                ((s.Seller.FirstName == null) ? "" : s.Seller.FirstName) + " " + s.Seller.LastName));
            //Task 7
            this.CreateMap<Category, CategoryByProductCountOutputModel>()
                .ForMember(cbp => cbp.Category, c => c.MapFrom(s => s.Name))
                .ForMember(cbp => cbp.ProductsCount, c => c.MapFrom(s => s.CategoryProducts.Count))
                .ForMember(cbp => cbp.AveragePrice, 
                    c => c.MapFrom(s => s.CategoryProducts.Average(cp => cp.Product.Price).ToString("F2")))
                .ForMember(cbp => cbp.TotalRevenue,
                    c => c.MapFrom(s => s.CategoryProducts.Sum(cp => cp.Product.Price).ToString("F2")));
        }
    }
}
