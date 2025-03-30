namespace DataEntity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SeatAssignments
    {
        [Key]
        public int AssignmentID { get; set; }

        public int? BookingID { get; set; }

        [StringLength(10)]
        public string SeatNumber { get; set; }

        public virtual Bookings Bookings { get; set; }
    }
}
