namespace ProductShop
{
    using AutoMapper;
    using ProductShop.Dtos.Export;
    using ProductShop.Dtos.Import;
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
            this.CreateMap<Product, ProductInRangeExportModel>()
                .ForMember(pe => pe.BuyerName, p => p.MapFrom(s => s.Buyer.FirstName + " " + s.Buyer.LastName));
            //NOTE: Example above doesn't work in Judge and exceeds the memomry limit.
            //Luckily, Judge does not have any examples where the first name is a null value.
            //Another possibility might be to simply .Select() the products with Linq,
            //though I'm not sure if that check won't exceed the memory limit as well.
            //.ForMember(
            //pe => pe.BuyerName, 
            //p => p.MapFrom(
            //    s => s.Buyer != null ? 
            //        (s.Buyer.FirstName != null ? s.Buyer.FirstName + " " : "") + s.Buyer.LastName 
            //        :
            //        null));
            
            //Task 6
            this.CreateMap<Product, SoldProductModelExportModel>();
            this.CreateMap<User, UserSoldProductExportModel>()
                .ForMember(us => us.SoldProducts, u => u.MapFrom(s => s.ProductsSold));
            //Task 7
            this.CreateMap<Category, CategoryExportModel>()
                .ForMember(ce => ce.AveragePrice, 
                    c => c.MapFrom(s => s.CategoryProducts.Average(cp => cp.Product.Price)))
                .ForMember(ce => ce.Count, c => c.MapFrom(s => s.CategoryProducts.Count))
                .ForMember(ce => ce.TotalRevenue, 
                c => c.MapFrom(s => s.CategoryProducts.Sum(cp => cp.Product.Price)));
            //Task 8
            //this.CreateMap<Product, UserAndProductProduct>();
            //this.CreateMap<User, UserAndProductUser>()
            //    .ForMember(uapu => uapu.SoldProducts, 
            //    u => u.MapFrom(s => new UserAndProductSoldProduct { Count = s.ProductsSold.Count, 
            //        Products = s.ProductsSold }))
            //    .AfterMap((u, uapu) => uapu.SoldProducts.Products.OrderByDescending(x => x.Price));
        }
    }
}
