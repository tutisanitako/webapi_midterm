using System.Collections.Generic;

namespace Domain.Entities
{
    public class Doctor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }

        // Navigation property for visits
        public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();

        // Parameterless constructor for EF6
        public Doctor()
        {
            // No validation in constructor; handled in service layer
        }
    }
}