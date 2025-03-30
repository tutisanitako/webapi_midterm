using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntity
{
    public class ShowtimeDto
    {
        public int ShowtimeID { get; set; }
        public int MovieID { get; set; }
        public int HallID { get; set; }
        public DateTime Showtime { get; set; }
        public decimal TicketPrice { get; set; }

        // Optional: Related data
        public string MovieTitle { get; set; }
        public string HallName { get; set; }
    }
}