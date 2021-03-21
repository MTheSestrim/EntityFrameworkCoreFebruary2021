namespace CarDealer.DTO.Task17
{
    using System.Collections.Generic;
    public class CarWithPartsOutputModel
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public long TravelledDistance { get; set; }
        public IEnumerable<PartOfCarOutputModel> parts { get; set; }
    }
}
