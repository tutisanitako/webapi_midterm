using System;
using System.Runtime.Serialization;

namespace DataEntity
{
    [DataContract]
    public class ShowtimeDto
    {
        [DataMember] public int ShowtimeID { get; set; }
        [DataMember] public int? MovieID { get; set; }
        [DataMember] public int? HallID { get; set; }
        [DataMember] public DateTime Showtime { get; set; }
        [DataMember] public decimal TicketPrice { get; set; }
        // Populated from navigation properties
        [DataMember] public string MovieTitle { get; set; }
        [DataMember] public string HallName { get; set; }
    }
}