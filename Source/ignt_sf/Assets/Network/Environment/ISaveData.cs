namespace ignt.sports.cricket.network
{
    public interface  ISaveData
    {

        public bool Save<T>(T data);


        public object Load<T>();
        
    }
}
