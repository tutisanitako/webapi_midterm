namespace DataEntity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Showtimes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Showtimes()
        {
            Bookings = new HashSet<Bookings>();
        }

        [Key]
        public int ShowtimeID { get; set; }

        public int? MovieID { get; set; }

        public int? HallID { get; set; }

        public DateTime Showtime { get; set; }

        public decimal TicketPrice { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bookings> Bookings { get; set; }

        public virtual Halls Halls { get; set; }

        public virtual Movies Movies { get; set; }
    }
}
