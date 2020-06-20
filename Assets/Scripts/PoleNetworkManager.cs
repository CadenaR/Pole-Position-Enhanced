using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Diagnostics;
using TMPro;
using System.Threading;
using UnityEngine.SceneManagement;

[AddComponentMenu("")]
public class PoleNetworkManager : NetworkRoomManager
{

    private SemaphoreSlim clientExit = new SemaphoreSlim(1);
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

    //When a client exits, roomSlots is updated, removing that client position
    // When there is only one player, session ends
    public override void OnStopClient()
    {
        base.OnStopClient();
        clientExit.Wait();
        for (int i = 0; i < roomSlots.Count; i++)
        {
            if(roomSlots[i] == null)
            {
                roomSlots.RemoveAt(i);
            }  
        }
        clientExit.Release();
        if (SceneManager.GetActiveScene().name == "RoomScene")
            return;

        if (roomSlots.Count == 1)
        {
            StopServer();
        }
    }

    #region Room

    //Called by SceneLoadedForPlayer, if classification lap is active, all players start at position 0, if not, round robin is used
    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
    {
        bool classif = roomSlots[0].GetComponent<PoleRoomPlayer>().classifLap;
        Transform startPos;
        // get start position depending on the selection of classification lap
        if(classif)
        {
            startPos = startPositions[0];
        }
        else
        {
            startPos = GetStartPosition();
        }
        // Instantiation of the on game player getting the prefab from the registered spawnable prefabs. The selected color (int) matches with the array position 
        // Red(0), Green (1), Yellow(2) & White(3) assigned at button clicked
        GameObject gamePlayer = startPos != null
            ? Instantiate(spawnPrefabs[roomPlayer.GetComponent<PoleRoomPlayer>().SelectedCar], startPos.position, startPos.rotation)
            : Instantiate(spawnPrefabs[roomPlayer.GetComponent<PoleRoomPlayer>().SelectedCar], Vector3.zero, Quaternion.identity);
        gamePlayer.name = playerPrefab.name;
        gamePlayer.GetComponent<PlayerInfo>().Name = roomPlayer.GetComponent<PoleRoomPlayer>().Name;
        int totalLaps = roomSlots[0].GetComponent<PoleRoomPlayer>().maxLap;        
        gamePlayer.GetComponent<PlayerInfo>().maxLap = totalLaps == 0 ? 3 : totalLaps;
        gamePlayer.GetComponent<PlayerInfo>().lap = 1;
        gamePlayer.GetComponent<SetupPlayer>().classifLap = classif;
        return gamePlayer;
    }

    #endregion

}
