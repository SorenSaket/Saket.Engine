using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine
{
    public interface IResource : IDisposable
    {
        public void Load();
    }
}
