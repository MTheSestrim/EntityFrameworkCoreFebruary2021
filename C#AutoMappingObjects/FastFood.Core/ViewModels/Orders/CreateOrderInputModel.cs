namespace FastFood.Core.ViewModels.Orders
{
    using FastFood.Models.Enums;
    public class CreateOrderInputModel
    {
        public string Customer { get; set; }

        public int ItemId { get; set; }

        public int EmployeeId { get; set; }

        public int Quantity { get; set; }

        public OrderType OrderType { get; set; }
    }
}
