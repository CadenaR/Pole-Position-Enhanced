using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Diagnostics;
using TMPro;

[AddComponentMenu("")]
public class PoleNetworkManager : NetworkRoomManager
{
    public bool clasif = true;

    public string PlayerName { get; set; }
     
    public void SetHostname(string hostname)
    {
        networkAddress = hostname;
    }

    //public ChatWindow chatWindow;

    public class CreatePlayerMessage : MessageBase
    {
        public string name;
    }

    #region Room

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
    {
        // get start position from base class
        Transform startPos = startPositions[0];
        // Instantiation of the on game player getting the prefab from the registered spawnable prefabs. The selected color (int) matches with the array position 
        // Red(0), Green (1), Yellow(2) & White(3) assigned at button clicked
        GameObject gamePlayer = startPos != null
            ? Instantiate(spawnPrefabs[roomPlayer.GetComponent<PoleRoomPlayer>().SelectedCar], startPos.position, startPos.rotation)
            : Instantiate(spawnPrefabs[roomPlayer.GetComponent<PoleRoomPlayer>().SelectedCar], Vector3.zero, Quaternion.identity);
        gamePlayer.name = playerPrefab.name;
        gamePlayer.GetComponent<PlayerInfo>().Name = roomPlayer.GetComponent<PoleRoomPlayer>().Name;
        gamePlayer.GetComponent<PlayerInfo>().lap = 1;
        int totalLaps = roomSlots[0].GetComponent<PoleRoomPlayer>().maxLap; 
        gamePlayer.GetComponent<PlayerInfo>().maxLap = totalLaps == null ? 3 : totalLaps < 3 ? 3 : totalLaps;

        return gamePlayer;
    }

    #endregion

}
