using System.ComponentModel.DataAnnotations;
using eTickets.V8.Data.Base;

namespace eTickets.V8.Models
{
    public class Actor : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Profile Picture is Required")]
        public string ProfilePictureURL { get; set; }
        [Required(ErrorMessage = "Full Name is Required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Full Name must between 3 and 50 chars")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Biography is Required")]
        public string Bio { get; set; }

        //relationship
        public List<Actor_Movie> Actors_Movies { get; set; }
    }
}
