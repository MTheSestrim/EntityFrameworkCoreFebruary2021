namespace CarDealer.DTO.Task18
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public class CustomerTotalSalesOutputModel
    {
        public string fullName { get; set; }

        public int boughtCars { get; set; }

        public decimal spentMoney { get; set; }
    }
}
