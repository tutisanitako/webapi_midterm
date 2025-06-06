using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class VisitUpdateDto
    {
        public int Id { get; set; } // ID is required for updates
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime VisitDate { get; set; }
        public decimal Fee { get; set; }
    }
}
