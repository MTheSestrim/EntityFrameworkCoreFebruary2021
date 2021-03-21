namespace CarDealer.DTOs.Import
{
    using System;
    using System.Xml.Serialization;

    [XmlType("Customer")]
    public class CustomerInputModel
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("birthDate")]
        public DateTime BirthDate { get; set; }

        [XmlElement("isYoungDriver")]
        public bool IsYoungDriver { get; set; }
    }
}

/*
 * <Customer>
        <name>Emmitt Benally</name>
        <birthDate>1993-11-20T00:00:00</birthDate>
        <isYoungDriver>true</isYoungDriver>
    </Customer>
 */
