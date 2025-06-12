using System.Collections.Generic;

namespace Domain.Entities
{
    public class Doctor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }

        public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();

        public Doctor()
        {
        }
    }
}