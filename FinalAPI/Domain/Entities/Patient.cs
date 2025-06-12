using System.Collections.Generic;
using System;

namespace Domain.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }

        public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();

        public Patient()
        {
        }
    }
}