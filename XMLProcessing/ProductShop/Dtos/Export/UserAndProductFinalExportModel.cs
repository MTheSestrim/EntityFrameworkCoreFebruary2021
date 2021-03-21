namespace ProductShop.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType]
    public class UserAndProductFinalExportModel
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]

        public UserAndProductUserExportModel[] Users { get; set; }
    }
}
