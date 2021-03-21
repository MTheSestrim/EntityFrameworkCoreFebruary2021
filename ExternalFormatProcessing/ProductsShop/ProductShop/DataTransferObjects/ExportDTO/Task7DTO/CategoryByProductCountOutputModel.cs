namespace ProductShop.DataTransferObjects.ExportDTO.Task7DTO
{
    public class CategoryByProductCountOutputModel
    {
        public string Category { get; set; }
        public int ProductsCount { get; set; }
        public string AveragePrice { get; set; }
        public string TotalRevenue { get; set; }
    }
}
