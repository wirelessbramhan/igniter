using System;
using UnityEngine;

namespace ignt.sports.cricket.network
{
    [CreateAssetMenu(fileName = "MatchDataSO", menuName = "NetworkManager/MatchDataSO")]
    public class MatchDataSO : ScriptableObject
    {
        public event Action OnMatchDataSuccess;
        
        public MatchData MatchData;
        public TextAsset DummyJson;
        public bool IsValid;

        public void GetMatchData()
        {
            FakeGetMatchData(DummyJson);
        }
        
        private async Awaitable<MatchData> GetMatchDataAsync(string url)
        {
            try
            {

                var httpClient = new NetworkHandler(new JsonSerializationOption());
                var result = await httpClient.Get<MatchData>(url);
                MatchData = result;
                IsValid = true;
                OnMatchDataSuccess?.Invoke();
                return result;
            }
            catch (Exception e)
            {
                Debug.Log(">" + e.Message + "< ");
                return null;
            }
            
        }

        private async Awaitable<MatchData> FakeGetMatchDataAsync(TextAsset json)
        {
            try
            {

                var httpClient = new FakeNetworkHandler(new JsonSerializationOption());
                var result = await httpClient.Get<MatchData>(json);
                MatchData = result;
                IsValid = true;
                OnMatchDataSuccess?.Invoke();
                return result;
            }
            catch (Exception e)
            {
                Debug.Log(">" + e.Message + "< ");
                return null;
            }
            
        }

        private void GetMatchData(string url)
        { 
            _ = GetMatchDataAsync(url);
        }

        private void FakeGetMatchData(TextAsset json)
        {
            _ = FakeGetMatchDataAsync(json);
        } 
        
    }

}
