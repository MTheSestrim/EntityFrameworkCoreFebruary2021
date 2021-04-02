using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P01_HospitalDatabase.Data.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Specialty { get; set; }

        public ICollection<Visitation> Visitations { get; set; }
    }
}
