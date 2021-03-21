namespace CarDealer.DTOs.Export
{
    using System.Xml.Serialization;

    [XmlType("suplier")]
    public class LocalSupplierOutputModel
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("parts-count")]
        public int PartsCount { get; set; }
    }
}
