using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piping.Test.TestData
{
    public class CarWithIInvariant : Car, IInvariant<CarWithIInvariant>
    {

        public const string InValidError = "Car State is Invalid";

        private bool isValid = true;

        public bool IsValid => isValid;

        public string Error => isValid ? "" : InValidError;

        public CarWithIInvariant SetError(string error = InValidError)
        {
            isValid = false;
            return this;
        }

        public CarWithIInvariant()
        {
        }

        public CarWithIInvariant(bool valid)
        {
            isValid = valid;
        }
    }
}
