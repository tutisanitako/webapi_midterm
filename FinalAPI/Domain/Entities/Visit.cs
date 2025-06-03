using System;

namespace FinalAPI.Domain.Entities
{
    public class Visit
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime VisitDate { get; set; }
        public decimal Fee { get; set; }

        // Navigation properties
        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }

        // Basic domain validation in constructor or setter
        public Visit()
        {
            if (Fee <= 0 || Fee >= 1000)
                throw new ArgumentException("Fee must be greater than 0 and less than 1000.");
            if (VisitDate > DateTime.Now)
                throw new ArgumentException("VisitDate cannot be in the future.");
        }
    }
}