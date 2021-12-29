using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Apartments.Models
{
    public class ApartmentVM
    {

        public Apartment Apartment { get; set; }
     
        public IEnumerable<Person> People { get; set; }

        public int vlasnikID { get; set; }
    }
}