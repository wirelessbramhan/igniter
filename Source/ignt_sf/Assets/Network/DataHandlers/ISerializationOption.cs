namespace ignt.sports.cricket.network
{
    public interface ISerializationOption
    {
        string ContentType { get; }
        T Deserialize<T>(string text);
    }
}