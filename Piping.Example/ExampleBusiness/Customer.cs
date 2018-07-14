using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Example.ExampleBusiness
{
    public class Customer : IKey
    {
        public int Id { get; }
        public Address Address { get; }
        public ECustomerType CustomerType { get; }

        public enum ECustomerType {
            Premium,
            Regular
        }

        public Customer() { }

        public Customer(int id, Address address, ECustomerType customerType) {
            Id = id;
            Address = address;
            CustomerType = customerType;
        }

        public int GetID()
        {
            return Id;
        }
    }
}
