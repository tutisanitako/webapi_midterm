using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class VisitQueryParameters
    {
        public int? DoctorId { get; set; }
        public DateTime? VisitDateFrom { get; set; }
        public DateTime? VisitDateTo { get; set; }
        public decimal? MinFee { get; set; }
        public decimal? MaxFee { get; set; }
        public string SortBy { get; set; } = "VisitDate"; // Default sort
        public string SortDirection { get; set; } = "asc"; // Default direction
        public int PageNumber { get; set; } = 1; // Default page number
        public int PageSize { get; set; } = 10; // Default page size
    }
}
