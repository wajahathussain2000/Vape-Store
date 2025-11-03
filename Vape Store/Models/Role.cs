using System;

namespace Vape_Store.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public bool IsSystem { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}


