//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVCPractice1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserRoleTbl
    {
        public UserRoleTbl()
        {
            this.UserTbls = new HashSet<UserTbl>();
        }
    
        public int id { get; set; }
        public string UserRole { get; set; }
    
        public virtual ICollection<UserTbl> UserTbls { get; set; }
    }
}
