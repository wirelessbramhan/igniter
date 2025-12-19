namespace ignt.sports.cricket.network
{
    public interface IMMPService
    {
        public void InitializeService();

        public void TrackEvent();

        public void TrackRevenue<T>(T revenueInfo);
    }
}