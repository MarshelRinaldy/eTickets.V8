using eTickets.V8.Data.Base;
using System.ComponentModel.DataAnnotations;

namespace eTickets.V8.Models
{
    public class Cinema : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        public string Logo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //relationship
        public List<Movie> Movies { get; set; }
    }
}
