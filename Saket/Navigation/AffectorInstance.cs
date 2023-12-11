using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Navigation
{
    [System.Serializable]
    public class AffectorInstance
    {
        public bool Valid
        {
            get
            {
                return (Affector != null) && Affector.Active;
            }
        }

        public AffectorInstance(VectorFieldAffector affector)
        {
            Affector = affector;
        }

        public VectorFieldAffector Affector { get; set; }

        public float[,] Heatmap { get; set; }

        public override int GetHashCode()
        {
            return Affector.GetHashCode();
        }
    }
}
