using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTickets.V8.Data.Base;
using eTickets.V8.Models;


namespace eTickets.V8.Data.Services
{
    public class ProducersService : EntityBaseRepository<Producer>, IProducersService
    {
        private readonly AppDbContext _context;
        public ProducersService(AppDbContext context) : base(context) { }
    }
}