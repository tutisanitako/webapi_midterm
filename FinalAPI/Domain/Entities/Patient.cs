using System.Collections.Generic;
using System;

namespace FinalAPI.Domain.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }

        // Navigation property for visits
        public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();

        // Basic domain validation in constructor or setter
        public Patient()
        {
            if (string.IsNullOrWhiteSpace(FullName))
                throw new ArgumentException("FullName cannot be empty or whitespace.");
            if (BirthDate > DateTime.Now)
                throw new ArgumentException("BirthDate cannot be in the future.");
        }
    }
}