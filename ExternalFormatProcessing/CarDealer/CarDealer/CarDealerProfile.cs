namespace CarDealer
{
    using AutoMapper;
    using CarDealer.DTO.Task10;
    using CarDealer.DTO.Task12;
    using CarDealer.DTO.Task13;
    using CarDealer.DTO.Task14;
    using CarDealer.DTO.Task15;
    using CarDealer.DTO.Task16;
    using CarDealer.DTO.Task17;
    using CarDealer.DTO.Task18;
    using CarDealer.DTO.Task9;
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
            this.CreateMap<Customer, CustomerOutputModel>()
                .ForMember(co => co.BirthDate, c => c.MapFrom(s => s.BirthDate.ToString("dd/MM/yyyy")));
            //Task 15
            this.CreateMap<Car, ToyotaMakeOutputModel>();
            //Task 16
            this.CreateMap<Supplier, LocalSupplierOutputModel>()
                .ForMember(ls => ls.PartsCount, s => s.MapFrom(x => x.Parts.Count));
            //Task 17
            this.CreateMap<Car, CarWithPartsOutputModel>()
                .ForMember(cwp => cwp.parts, c => c.MapFrom(s => s.PartCars.Select(pc => pc.Part)));
            this.CreateMap<Part, PartOfCarOutputModel>()
                .ForMember(pcp => pcp.Price, p => p.MapFrom(s => s.Price.ToString("F2")));
        }
    }
}
