using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVCPractice1.Models
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Enter Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Choose Department")]
        public Nullable<int> DepartmentId { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string imagePath { get; set; }


        [NotMapped]
        public HttpPostedFileBase ImageUpload { get; set; }
        public string DepartmentName { get; set; }
        public string Address { get; set; }
        public string SiteName { get; set; }
    }
}