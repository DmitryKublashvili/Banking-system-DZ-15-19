using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DZ_13_Banking_system
{
    class NotEnoughMoneyException : Exception
    {

        public NotEnoughMoneyException(string msg) : base(msg)
        {
            
        }

    }
}
