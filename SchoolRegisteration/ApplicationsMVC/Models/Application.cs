//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApplicationsMVC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Application
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string address { get; set; }
        [Required(ErrorMessage = "date is required")]
        public System.DateTime dob { get; set; }
        [Required(ErrorMessage = "Age is required")]
        [Range(5,15)]
        public int age { get; set; }
        public int branch_id { get; set; }
        public int classid { get; set; }
        public string category { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual ProcessedApplication ProcessedApplication { get; set; }
    }
}
