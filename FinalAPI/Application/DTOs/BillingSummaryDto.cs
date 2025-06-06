using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class BillingSummaryDto
    {
        public int PatientId { get; set; }
        public string PatientFullName { get; set; } = string.Empty;
        public decimal TotalPaid { get; set; }
    }
}
