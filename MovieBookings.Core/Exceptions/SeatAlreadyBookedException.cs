using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBookings.Core.Exceptions
{
    public class SeatAlreadyBookedException : Exception
    {
        public SeatAlreadyBookedException()
        {
                
        }

        public SeatAlreadyBookedException(string message) : base(message)
        {

        }
    }
}
