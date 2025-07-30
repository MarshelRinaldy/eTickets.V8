using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using eTickets.V8.Models;

namespace eTickets.V8.Data.ViewModels
{
    public class CinemaVM
    {
        public IFormFile? Logo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}