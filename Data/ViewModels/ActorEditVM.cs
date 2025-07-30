using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eTickets.V8.Data.ViewModels
{
    public class ActorEditVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Current Profile Picture")]
        public string? CurrentProfilePictureURL { get; set; }

        [Display(Name = "New Profile Picture (Optional)")]
        public IFormFile? NewProfilePictureURL { get; set; }

        [Display(Name = "Biography")]
        public string Bio { get; set; }
    }
}