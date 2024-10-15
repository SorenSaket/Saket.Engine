using Saket.Serialization;

namespace Saket.Engine.Documents;

/// <summary>
/// All data to be edited is contained asa document
/// The "abstract memory format" of files.
/// </summary>
public abstract class Document : ISerializable
{
    public abstract void SaveToPath(string path);

    public abstract void LoadFromPath(string path);

    public abstract void Serialize(ISerializer serializer);
}
