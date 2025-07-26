using System.ComponentModel.DataAnnotations;
using eTickets.V8.Data.Base;

namespace eTickets.V8.Models
{
    public class Producer : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        public string ProfilePictureURL { get; set; }
        public string FullName { get; set; }
        public string Bio { get; set; }

        //Relationship
        public List<Movie> Movies { get; set; }
    }
}
