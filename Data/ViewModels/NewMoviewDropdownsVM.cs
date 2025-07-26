using eTickets.V8.Models;

namespace eTickets.V8.Data.ViewModels
 
{
    public class NewMoviewDropdownsVM
    {

        public NewMoviewDropdownsVM()
        {
            Producers = new List<Producer>();
            Cinemas = new List<Cinema>();
            Actors = new List<Actor>();

        }
        public List<Producer> Producers { get; set; }
        public List<Cinema> Cinemas { get; set; }
        public List<Actor> Actors { get; set; }
    }   
}
