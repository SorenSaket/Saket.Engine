using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Saket.Engine;

/// <summary>
/// Derive you application entry point from here. 
/// Timing and calling
/// </summary>
public abstract class Application
{
    /// <summary>
    /// The time elapsed in seconds since the last time Update() was called.
    /// </summary>
    public double DeltaTime { get; protected set; }
    /// <summary>
    /// Total number of seconds since the application has started.
    /// </summary>
    public double TotalElapsedTime { get; protected set; }
    /// <summary>
    /// Total number of ticks since the application has started.
    /// </summary>
    public long TotalElapsedTicks { get; protected set; }
    /// <summary>
    /// The total number of times Update() has been called.
    /// </summary>
    public uint Frame { get; protected set; }

    protected bool shouldTerminate;

    protected Stopwatch timer;

    long ticksLast;

    // These are vitual instead of abstract because abstract methods require overloading. These are optional.

    #region public methods
    /// <summary>
    /// Start the application. Timing starts and Update functions are called
    /// </summary>
    public virtual void Run()
    {
        //Thread t = new Thread(DoRun);
        //t.Start();

        DoRun();
    }
    /// <summary>
    /// The main function for logic get called continuously unbound.
    /// </summary>
    public virtual void Update() { }
    /// <summary>
    /// Get called at a fixed interval defined with
    /// </summary>
    public virtual void FixedUpdate() { }
    /// <summary>
    /// Close the application
    /// </summary>
    public virtual void Termiate()
    {
        shouldTerminate = true;
    }
    #endregion



    //
    private void DoRun()
    {
        timer = Stopwatch.StartNew();

        // The application keeps calling update until Terminate() is called.
        while (!shouldTerminate)
        {
            TotalElapsedTicks   += timer.ElapsedTicks;
            
            DeltaTime = TimeSpan.FromTicks(timer.ElapsedTicks).TotalSeconds;

            TotalElapsedTime += DeltaTime;
            
            timer.Restart();
            timer.Start();

            // Run the frame
            Update();

            Frame++;
        }
    }

 
}