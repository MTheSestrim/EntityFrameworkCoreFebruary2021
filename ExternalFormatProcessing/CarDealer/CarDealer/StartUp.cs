namespace CarDealer
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using CarDealer.Data;
    using CarDealer.DTO.Task10;
    using CarDealer.DTO.Task11;
    using CarDealer.DTO.Task12;
    using CarDealer.DTO.Task13;
    using CarDealer.DTO.Task14;
    using CarDealer.DTO.Task15;
    using CarDealer.DTO.Task16;
    using CarDealer.DTO.Task17;
    using CarDealer.DTO.Task18;
    using CarDealer.DTO.Task9;
    using CarDealer.Models;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    public class StartUp
    {
        private static IMapper mapper;
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();
            //
            //string suppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");
            //string partsJson = File.ReadAllText("../../../Datasets/parts.json");
            //string carsJson = File.ReadAllText("../../../Datasets/cars.json");
            //string customersJson = File.ReadAllText("../../../Datasets/customers.json");
            //string salesJson = File.ReadAllText("../../../Datasets/sales.json");

            //ImportSuppliers(context, suppliersJson);
            //ImportParts(context, partsJson);
            //ImportCars(context, carsJson);
            //ImportCustomers(context, customersJson);
            //ImportSales(context, salesJson);

            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            var dtoSuppliers = JsonConvert.DeserializeObject<IEnumerable<SupplierInputModel>>(inputJson);

            var suppliers = mapper.Map<IEnumerable<Supplier>>(dtoSuppliers);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            var supplierIds = context.Suppliers.Select(s => s.Id);

            var dtoParts = JsonConvert.DeserializeObject<IEnumerable<PartInputModel>>(inputJson);

            var parts = mapper.Map<IEnumerable<Part>>(dtoParts).Where(p => supplierIds.Contains(p.SupplierId));

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            var dtoCars = JsonConvert.DeserializeObject<IEnumerable<CarInputModel>>(inputJson);

            var cars = new HashSet<Car>();

            foreach (var car in dtoCars)
            {
                var currentCar = new Car
                {
                    Make = car.Make,
                    Model = car.Model,
                    TravelledDistance = car.TravelledDistance
                };

                foreach (var partId in car.PartsId.Distinct())
                {
                    var currentPartCar = new PartCar()
                    {
                        PartId = partId,
                        CarId = currentCar.Id
                    };
                    currentCar.PartCars.Add(currentPartCar);
                }

                cars.Add(currentCar);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count()}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            var dtoCustomers = JsonConvert.DeserializeObject<IEnumerable<CustomerInputModel>>(inputJson);

            var customers = mapper.Map<IEnumerable<Customer>>(dtoCustomers);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            var dtoSales = JsonConvert.DeserializeObject<IEnumerable<SaleInputModel>>(inputJson);

            var sales = mapper.Map<IEnumerable<Sale>>(dtoSales);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            InitializeAutoMapper();

            var customers = context
                .Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .ProjectTo<CustomerOutputModel>(mapper.ConfigurationProvider)
                .ToList();

            string result = JsonConvert.SerializeObject(customers);

            return result;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            InitializeAutoMapper();

            var cars = context
                .Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ProjectTo<ToyotaMakeOutputModel>(mapper.ConfigurationProvider)
                .ToList();

            string result = JsonConvert.SerializeObject(cars);

            return result;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            InitializeAutoMapper();

            var suppliers = context
                .Suppliers
                .Where(s => !s.IsImporter)
                .ProjectTo<LocalSupplierOutputModel>(mapper.ConfigurationProvider)
                .ToList();

            string result = JsonConvert.SerializeObject(suppliers);

            return result;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            InitializeAutoMapper();

            var cars = context
                .Cars
                .ProjectTo<CarWithPartsOutputModel>(mapper.ConfigurationProvider)
                .ToList();

            var outputObjects = cars.Select(c => new
            {
                car = new
                {
                    c.Make,
                    c.Model,
                    c.TravelledDistance
                },
                c.parts
            });

            string result = JsonConvert.SerializeObject(outputObjects, Formatting.Indented);

            return result;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            InitializeAutoMapper();

            var customers = context
                .Customers
                .Where(c => c.Sales.Any(s => s.Car != null))
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    spentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(pc => pc.Part.Price))
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars)
                .ToList();

            string result = JsonConvert
                .SerializeObject(
                    customers);

            return result;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            InitializeAutoMapper();

            var customers = context
                .Sales
                .Take(10)
                .Select(c => new
                {
                    car = new
                    {
                        c.Car.Make,
                        c.Car.Model,
                        c.Car.TravelledDistance,
                    },
                    customerName = c.Customer.Name,
                    Discount = c.Discount.ToString("F2"),
                    price = c.Car.PartCars.Sum(pc => pc.Part.Price).ToString("F2"),
                    priceWithDiscount = 
                        (c.Car.PartCars.Sum(pc => pc.Part.Price) * ((100 - c.Discount) / 100)).ToString("F2")
                })
                .ToList();

            string result = JsonConvert.SerializeObject(customers);

            return result;
        }
        private static void InitializeAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            mapper = config.CreateMapper();
        }
    }
}