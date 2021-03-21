namespace CarDealer
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using CarDealer.Data;
    using CarDealer.DTOs.Export;
    using CarDealer.DTOs.Import;
    using CarDealer.Models;
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
            var context = new CarDealerContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //string suppliersXML = File.ReadAllText("../../../Datasets/suppliers.xml");
            //string partsXML = File.ReadAllText("../../../Datasets/parts.xml");
            //string carsXML = File.ReadAllText("../../../Datasets/cars.xml");
            //string customersXML = File.ReadAllText("../../../Datasets/customers.xml");
            //string salesXML = File.ReadAllText("../../../Datasets/sales.xml");

            //ImportSuppliers(context, suppliersXML);
            //ImportParts(context, partsXML);
            //ImportCars(context, carsXML);
            //ImportCustomers(context, customersXML);
            //ImportSales(context, salesXML);

            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            InitializeAutoMapper();

            var serializer = new XmlSerializer(
                typeof(SupplierInputModel[]), 
                new XmlRootAttribute("Suppliers"));

            var dtoSuppliers = serializer.Deserialize(new StringReader(inputXml)) as SupplierInputModel[];

            var suppliers = mapper.Map<IEnumerable<Supplier>>(dtoSuppliers);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported { suppliers.Count() }";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            InitializeAutoMapper();

            var serializer = new XmlSerializer(
                typeof(PartInputModel[]),
                new XmlRootAttribute("Parts"));

            var supplierIds = context.Suppliers.Select(s => s.Id);

            var dtoParts = serializer.Deserialize(new StringReader(inputXml)) as PartInputModel[];

            var parts = mapper.Map<IEnumerable<Part>>(dtoParts).Where(p => supplierIds.Contains(p.SupplierId));

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported { parts.Count() }";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(CarInputModel[]), new XmlRootAttribute("Cars"));

            var dtoCars = serializer.Deserialize(new StringReader(inputXml)) as CarInputModel[];

            var cars = new List<Car>();

            var existingPartIds = context.Parts.Select(p => p.Id);

            foreach (var dtoCar in dtoCars)
            {
                var parts = dtoCar.Parts.Select(p => p.Id).Distinct().Intersect(existingPartIds);

                var car = new Car
                {
                    Make = dtoCar.Make,
                    Model = dtoCar.Model,
                    TravelledDistance = dtoCar.TravelledDistance
                };

                foreach (var part in parts)
                {
                    var partCar = new PartCar
                    {
                        PartId = part
                    };

                    car.PartCars.Add(partCar);
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            InitializeAutoMapper();

            var serializer = new XmlSerializer(typeof(CustomerInputModel[]), new XmlRootAttribute("Customers"));

            var dtoCustomers = serializer.Deserialize(new StringReader(inputXml)) as CustomerInputModel[];

            var customers = mapper.Map<IEnumerable<Customer>>(dtoCustomers);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            InitializeAutoMapper();

            var serializer = new XmlSerializer(typeof(SaleInputModel[]), new XmlRootAttribute("Sales"));

            var dtoSales = serializer.Deserialize(new StringReader(inputXml)) as SaleInputModel[];

            var existingCarIds = context.Cars.Select(c => c.Id);

            var sales = mapper.Map<IEnumerable<Sale>>(dtoSales.Where(s => existingCarIds.Contains(s.CarId)));

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            InitializeAutoMapper();

            var cars = context
                .Cars
                .Where(c => c.TravelledDistance > 2_000_000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ProjectTo<CarWithDistanceOutputModel>(mapper.ConfigurationProvider)
                .ToArray();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            var serializer = 
                new XmlSerializer(typeof(CarWithDistanceOutputModel[]), new XmlRootAttribute("cars"));

            StringBuilder result = new StringBuilder();
            serializer.Serialize(new StringWriter(result), cars, namespaces);

            return result.ToString().Trim();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            InitializeAutoMapper();

            var cars = context
                .Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ProjectTo<BMWCarOutputModel>(mapper.ConfigurationProvider)
                .ToArray();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            var serializer = new XmlSerializer(typeof(BMWCarOutputModel[]), new XmlRootAttribute("cars"));

            StringBuilder result = new StringBuilder();
            serializer.Serialize(new StringWriter(result), cars, namespaces);

            return result.ToString().Trim();
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            InitializeAutoMapper();

            var suppliers = context
                .Suppliers
                .Where(s => !s.IsImporter)
                .ProjectTo<LocalSupplierOutputModel>(mapper.ConfigurationProvider)
                .ToArray();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            var serializer = 
                new XmlSerializer(typeof(LocalSupplierOutputModel[]), new XmlRootAttribute("suppliers"));

            StringBuilder result = new StringBuilder();
            serializer.Serialize(new StringWriter(result), suppliers, namespaces);

            return result.ToString().Trim();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithParts = context
                .Cars
                .Select(c => new CarPartCarOutputModel 
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars.Select(pc => new CarPartPartOutputModel 
                        {
                            Name = pc.Part.Name,
                            Price = pc.Part.Price,
                        }).OrderByDescending(p => p.Price).ToArray()
                })
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ToArray();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            var serializer =
                new XmlSerializer(typeof(CarPartCarOutputModel[]), new XmlRootAttribute("cars"));

            StringBuilder result = new StringBuilder();
            serializer.Serialize(new StringWriter(result), carsWithParts, namespaces);

            return result.ToString().Trim();
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context
                .Customers
                .Where(c => c.Sales.Any())
                .Select(c => new CustomerTotalSaleOutputModel
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c
                        .Sales
                        .Select(s => s.Car)
                        .SelectMany(s => s.PartCars.Select(pc => pc.Part))
                        .Sum(p => p.Price),
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            var serializer = new XmlSerializer(typeof(CustomerTotalSaleOutputModel[]),
                new XmlRootAttribute("customers"));

            var result = new StringBuilder();
            serializer.Serialize(new StringWriter(result), customers, namespaces);

            return result.ToString();
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            InitializeAutoMapper();

            var sales = context
                .Sales
                .ProjectTo<SaleWithDiscountOutputModel>(mapper.ConfigurationProvider)
                .ToArray();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            var serializer = new XmlSerializer(typeof(SaleWithDiscountOutputModel[]),
                new XmlRootAttribute("sales"));

            var result = new StringBuilder();
            serializer.Serialize(new StringWriter(result), sales, namespaces);

            return result.ToString();
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