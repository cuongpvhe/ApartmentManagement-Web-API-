using System;
using System.Collections.Generic;

namespace ApartmentManagement.Models
{
    public partial class Manager
    {
        public Manager()
        {
            Buildings = new HashSet<Building>();
            Services = new HashSet<Service>();
        }

        public int ManagerId { get; set; }
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int Role { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordExpiry { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<Building> Buildings { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }
}
