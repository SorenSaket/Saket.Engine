namespace Saket;

// Generic caching system
// Able to propergate calculcations and revalidations
// Lazy recalculation based upon request or eager based on underlying data change


// Cache: Storing some arbitrary data
/*

/// <summary>
/// 
/// </summary>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TOutput"></typeparam>
public struct CachedReference<TInput,TOutput>
{
    public Cache<TInput, TOutput> cache;
    public int version;

    public TOutput GetData(TInput input, out bool dataChanged)
    {
        if(cache.version > version)
        {
            // The data is old
            dataChanged = true;
            version = cache.version;

        }

        dataChanged = false;
    }

    public Action<TOutput> OnCacheRefreshed;
}

public enum CacheRecalcuatation
{
    Lazy,
    Eager
}

public class Cache<TInput, TOuput>
{
    public int version;
    public TOuput cachedOutput;
    public Action<TInput, TOuput> Action;

    /// <summary>
    /// Only recalculate
    /// </summary>
    public CacheRecalcuatation condition;


    public void Invalidate(TInput newInput)
    {
        Action?.Invoke();
    }

    public void GetReference()
    {

    }

    public Cache()
    {
    }
}

*/