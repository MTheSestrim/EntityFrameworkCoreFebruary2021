namespace CarDealer
{
    using AutoMapper;
    using CarDealer.DTOs.Export;
    using CarDealer.DTOs.Import;
    using CarDealer.Models;
    using System.Linq;

    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            //Task 9
            this.CreateMap<SupplierInputModel, Supplier>();
            //Task 10
            this.CreateMap<PartInputModel, Part>();
            //Task 12
            this.CreateMap<CustomerInputModel, Customer>();
            //Task 13
            this.CreateMap<SaleInputModel, Sale>();
            //Task 14
            this.CreateMap<Car, CarWithDistanceOutputModel>();
            //Task 15
            this.CreateMap<Car, BMWCarOutputModel>();
            //Task 16
            this.CreateMap<Supplier, LocalSupplierOutputModel>();
            //Task 19
            this.CreateMap<Car, SaleWithDiscountCarOutputModel>();
            this.CreateMap<Sale, SaleWithDiscountOutputModel>()
                .ForMember(swd => swd.Price, 
                s => s.MapFrom(x => x.Car.PartCars.Select(pc => pc.Part).Sum(p => p.Price)))
                .ForMember(swd => swd.PriceWithDiscount,
                s => s.MapFrom(
                    x => (x.Car.PartCars.Select(pc => pc.Part).Sum(p => p.Price) 
                        * ((100 - x.Discount) / 100))
                        //G29 removes trailing zeroes
                        .ToString("G29")
                        ));
        }
    }
}
