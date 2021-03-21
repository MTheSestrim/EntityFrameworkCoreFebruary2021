namespace CarDealer.DTO.Task11
{
    using System.Collections.Generic;
    public class CarInputModel
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public long TravelledDistance { get; set; }
        public IEnumerable<int> PartsId { get; set; }
    }
}
