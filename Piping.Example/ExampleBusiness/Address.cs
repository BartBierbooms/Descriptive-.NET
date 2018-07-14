using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Example.ExampleBusiness
{
    public class Address : IKey
    {
        public Address(int ID, string street, string town, string zipCode, string country) {
            this.ID = ID;
            Street = street;
            Town = town;
            ZipCode = zipCode;
            Country = country;
        }

        public int ID { get; }
        public string Street { get; set; }
        public string Town { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }

        public int GetID()
        {
            return this.ID;
        }
    }
}
