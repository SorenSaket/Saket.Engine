using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Net.Realtime
{
    public class RTInput_Client<ClientInput>
    {
        /// <summary>
        /// Buffer of unacknowledged input
        /// </summary>
        public Queue<ClientInput> buffer_input = new();



    }
}
