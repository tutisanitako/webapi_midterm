using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class VisitReadDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientFullName { get; set; } = string.Empty; // Denormalized for convenience
        public DateTime PatientBirthDate { get; set; } // Denormalized for convenience
        public int DoctorId { get; set; }
        public string DoctorFullName { get; set; } = string.Empty; // Denormalized for convenience
        public string DoctorSpecialization { get; set; } = string.Empty; // Denormalized for convenience
        public DateTime VisitDate { get; set; }
        public decimal Fee { get; set; }
    }
}
