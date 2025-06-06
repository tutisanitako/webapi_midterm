using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class DoctorVisitsSummaryDto
    {
        public int DoctorId { get; set; }
        public string DoctorFullName { get; set; } = string.Empty;
        public string DoctorSpecialization { get; set; } = string.Empty;
        public int TotalVisits { get; set; }
    }
}
