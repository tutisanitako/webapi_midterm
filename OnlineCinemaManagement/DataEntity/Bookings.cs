namespace DataEntity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Bookings
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Bookings()
        {
            SeatAssignments = new HashSet<SeatAssignments>();
        }

        [Key]
        public int BookingID { get; set; }

        public int? UserID { get; set; }

        public int? ShowtimeID { get; set; }

        public int? NumberOfTickets { get; set; }

        public DateTime? BookingDate { get; set; }

        public virtual Showtimes Showtimes { get; set; }

        public virtual Users Users { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SeatAssignments> SeatAssignments { get; set; }
    }
}
