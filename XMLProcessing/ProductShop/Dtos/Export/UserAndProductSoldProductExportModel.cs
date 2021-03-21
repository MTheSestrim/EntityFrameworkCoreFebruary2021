namespace ProductShop.Dtos.Export
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType("SoldProducts")]
    public class UserAndProductSoldProductExportModel
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("products")]
        public UserAndProductProductExportModel[] Products { get; set; }
    }
}