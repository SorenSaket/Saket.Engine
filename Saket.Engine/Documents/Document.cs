using Saket.Serialization;

namespace Saket.Engine.Documents;

/// <summary>
/// All data to be edited is contained asa document
/// The "abstract memory format" of files.
/// </summary>
public abstract class Document : ISerializable
{
    public string? Name {  get; set; }


    public abstract void SaveToPath(string path_full);

    public abstract void LoadFromPath(string path_full);

    public abstract void Serialize(ISerializer serializer);
}
