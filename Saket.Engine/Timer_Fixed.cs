using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine;

public class Timer_Fixed
{


    public event Action Update;



    public bool IsRunningSlow { get; protected set; }


    public float TargetFrequency = 60;

    Stopwatch timer;


    public void UpdateTimer()
    {



    }

}
