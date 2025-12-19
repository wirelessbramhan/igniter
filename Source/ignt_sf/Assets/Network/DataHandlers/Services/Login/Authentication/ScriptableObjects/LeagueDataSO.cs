using System;
using UnityEngine;

namespace ignt.sports.cricket.network
{
    [CreateAssetMenu(fileName = "LeagueDataSO", menuName = "NetworkManager/LeagueDataSO")]
    public class LeagueDataSO : ScriptableObject
    {
        public event Action OnLeagueDataSuccess;
        public LeagueData LeagueData;
        public TextAsset DummyJson;
        public bool IsValid;


        public void GetLeagueData()
        {
            FakeGetLeagueData(DummyJson);
        }
        
        private async Awaitable<LeagueData> GetLeagueDataAsync(string url)
        {
            try
            {

                var httpClient = new NetworkHandler(new JsonSerializationOption());
                var result = await httpClient.Get<LeagueData>(url);
                LeagueData = result;
                IsValid = true;
                OnLeagueDataSuccess?.Invoke();
                return result;
            }
            catch (Exception e)
            {
                Debug.Log(">" + e.Message + "< ");
                return null;
            }
            
        }

        private async Awaitable<LeagueData> FakeGetLeagueDataAsync(TextAsset json)
        {
            try
            {

                var httpClient = new FakeNetworkHandler(new JsonSerializationOption());
                var result = await httpClient.Get<LeagueData>(json);
                LeagueData = result;
                IsValid = true;
                OnLeagueDataSuccess?.Invoke();
                return result;
            }
            catch (Exception e)
            {
                Debug.Log(">" + e.Message + "< ");
                return null;
            }
            
        }

        private void GetLeagueData(string url)
        {
            _ = GetLeagueDataAsync(url);
        }
        
        private void FakeGetLeagueData(TextAsset json)
        {
            _ = FakeGetLeagueDataAsync(json);
        }



    }
}
