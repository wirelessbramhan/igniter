using System.Collections.Generic;
using UnityEngine;

namespace com.krafton.fantasysports.core
{
    [CreateAssetMenu(fileName = "DummyPlayers_default", menuName = "UI/Dummy Data/Players", order = 0)]
    public class PlayerDataSO : ScriptableObject
    {
        public List<PlayerData> DummyData;
        public int PlayersToSpawn;

        [ContextMenu("Add Random Players")]
        public void AddPlayersRandom()
        {
            DummyData = new List<PlayerData>();

            for (int i = 0; i < PlayersToSpawn; i++)
            {
                PlayerData newPlayer = new("Player" + (i + 1), (PlayerType)Random.Range(0, 3), Random.Range(5, 9), Random.Range(1, 9) + 0.5f, (i % 3 == 0));

                DummyData.Add(newPlayer);
            }
        }
    }
}
