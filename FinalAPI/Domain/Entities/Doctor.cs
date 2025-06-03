using System.Collections.Generic;
using System;

namespace FinalAPI.Domain.Entities
{
    public class Doctor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }

        // Navigation property for visits
        public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();

        // Basic domain validation in constructor or setter
        public Doctor()
        {
            if (string.IsNullOrWhiteSpace(FullName))
                throw new ArgumentException("FullName cannot be empty or whitespace.");
            if (string.IsNullOrWhiteSpace(Specialization))
                throw new ArgumentException("Specialization cannot be empty or whitespace.");
        }
    }
}