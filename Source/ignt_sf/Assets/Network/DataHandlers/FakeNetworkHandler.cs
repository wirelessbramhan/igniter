using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using ignt.sports.cricket.network;

namespace ignt.sports.cricket.network
{
    public class FakeNetworkHandler
    {
        private readonly ISerializationOption _serializationOption;

        public FakeNetworkHandler(ISerializationOption serializationOption)
        {
            _serializationOption = serializationOption;
        }

        public async Awaitable<TResultType> Get<TResultType>(TextAsset json)
        {
            try
            {
                await Awaitable.WaitForSecondsAsync(1f);
                var result = _serializationOption.Deserialize<TResultType>(json.text);

                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"{nameof(Get)} failed: {ex.Message}");
                return default;
            }
        }
        public async Awaitable<TResultType> Post<TResultType>(TextAsset json, string form)
        {
            try
            {
                await Awaitable.WaitForSecondsAsync(1f);
                var result = _serializationOption.Deserialize<TResultType>(json.text);
   

                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"{nameof(Get)} failed: {ex.Message}");
                return default;
            }
        }
    }
}