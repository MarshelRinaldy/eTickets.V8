using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace eTickets.V8.Models
{
    public class ApplicationUser : IdentityUser
    {

        //its an additional column for IdentityUser
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

    }
}