using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Saket.Engine
{
    /// <summary>
    /// Derive you application entry point from here
    /// </summary>
    public abstract class Application
    {
        protected bool shouldTerminate;

        public virtual void Update(float deltaTime) { }
        

        public void Run()
        {
            //Thread t = new Thread(DoRun);
            //t.Start();

            DoRun();
        }
        
        private void DoRun()
        {
            var timer = Stopwatch.StartNew();
            while (!shouldTerminate)
            {
                Update((float)timer.Elapsed.TotalSeconds);
                timer.Restart();
                timer.Start();
            }
        }

        public void Termiate()
        {
            shouldTerminate = true;
        }
    }
}