using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine
{
    public class ApplicationTime
    {
        public double DeltaTime { get; protected set; }

        public double TotalElapsedTime { get; protected set; }

        public long TotalElapsedTicks { get; protected set; }
    }
}