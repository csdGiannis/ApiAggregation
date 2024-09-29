using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAggregation.Domain.DomainModels
{
    public class Library
    {
        public int TotalResults { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
