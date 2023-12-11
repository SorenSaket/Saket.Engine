using Saket.Navigation.VectorField;
using System.Numerics;

namespace Saket.Navigation;

public struct Affector
{
    /// <summary>
    /// Whether or not this should act as the main affector
    /// </summary>
    public bool Main { get; set; } = false;

    /// <summary>
    /// Whether or not
    /// </summary>
    public bool Active = true;
    
    /// <summary>
    /// 
    /// </summary>
    public bool Blocking = true;

    /// <summary>
    /// The strength of the affector
    /// </summary>
    public float Stength = 1;

    /// <summary>
    /// The maximum radius of the affector
    /// </summary>
    public float Radius = 3;

    /// <summary>
    /// The Falloff fuction to use
    /// </summary>
    public Falloff Falloff = 0;
    
    /// <summary>
    /// Modifier for the falloff
    /// </summary>
    public float FalloffValue = 1;
    
    /// <summary>
    /// World position of the affector
    /// </summary>
    public Vector3 Position;

    public Affector()
    {
    }
}
