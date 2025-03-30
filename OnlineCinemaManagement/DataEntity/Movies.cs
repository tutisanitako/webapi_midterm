namespace DataEntity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Movies
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Movies()
        {
            Showtimes = new HashSet<Showtimes>();
        }

        [Key]
        public int MovieID { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public int? Genre { get; set; }

        public int? Duration { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ReleaseDate { get; set; }

        [StringLength(50)]
        public string Language { get; set; }

        public virtual Genres Genres { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Showtimes> Showtimes { get; set; }
    }
}
