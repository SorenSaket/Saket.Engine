using Saket.Navigation.VectorField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Navigation;

[System.Serializable]
public class VectorFieldAffector
{
    public event Action OnValuesChanged;

    public bool Main { get; set; } = false;


    private bool active = true;

    private bool blocking = true;
    
    private float stength = 1;
    
    private float radius = 3;
    
    /// <summary>
    /// The Falloff fuction to use
    /// </summary>
    private Falloff falloff = 0;
    
    private float falloffValue = 1;

    public Vector3 position;

    public VectorFieldAffector() { }

    public VectorFieldAffector(Vector3 position, bool main, bool active, bool blocking, float stength, float radius, Falloff falloff, float falloffValue)
    {
        this.position = position;
        Main = main;
        this.active = active;
        this.blocking = blocking;
        this.stength = stength;
        this.radius = radius;
        this.falloff = falloff;
        this.falloffValue = falloffValue;
    }

    public bool Active { get { return active; } set { if (active != value) { active = value; OnValuesChanged?.Invoke(); } } }
    public bool Blocking { get { return blocking; } set { if (blocking != value) { blocking = value; OnChange(); } } }
    public float Stength { get { return stength; } set { if (stength != value) { stength = value; OnChange(); } } }
    public float Radius { get { return radius; } set { if (radius != value) { radius = value; OnChange(); } } }
    public Falloff Falloff { get { return falloff; } set { if (falloff != value) { falloff = value; OnChange(); } } }
    public float FalloffValue { get { return falloffValue; } set { if (falloffValue != value) { falloffValue = value; OnChange(); } } }
    public Vector3 Position { get { return position; } set { if (position != value) { position = value; OnChange(); } } }

    public void OnChange()
    {
        if (!active)
            return;
        OnValuesChanged?.Invoke();
    }


}


