using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PatientUpdateDto
    {
        public int Id { get; set; } // ID is required for updates
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
    }
}
