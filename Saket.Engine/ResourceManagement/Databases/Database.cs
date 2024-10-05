﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.ResourceManagement.Databases
{
    public abstract class Database
    {
        public bool IsInitialized => initialized;
        protected bool initialized;
        public abstract void Initialize();
        public abstract HashSet<string> AvaliableResources { get; }
        public abstract Stream? TryGetStream(string name);
    }
}