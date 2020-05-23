using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Diagnostics;
using TMPro;

[AddComponentMenu("")]
public class PoleNetworkManager : NetworkRoomManager
{
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

    /*public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreatePlayer);
    }

    /*public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        // tell the server to create a player with this name
        conn.Send(new CreatePlayerMessage { name = PlayerName });
    }

    private void OnCreatePlayer(NetworkConnection connection, CreatePlayerMessage createPlayerMessage)
    {
        // create a gameobject using the name supplied by client
        Transform startPos = GetStartPosition();
        GameObject playergo = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        playergo.GetComponent<PlayerInfo>().Name = createPlayerMessage.name;

        // set it as the player
        NetworkServer.AddPlayerForConnection(connection, playergo);

        //chatWindow.gameObject.SetActive(true);
    }*/

    #region Room

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
    {
        // get start position from base class
        Transform startPos = GetStartPosition();
        // Instantiation of the on game player getting the prefab from the registered spawnable prefabs. The selected color (int) matches with the array position 
        // Red(0), Green (1), Yellow(2) & White(3) assigned at button clicked
        GameObject gamePlayer = startPos != null
            ? Instantiate(spawnPrefabs[roomPlayer.GetComponent<PoleRoomPlayer>().SelectedCar], startPos.position, startPos.rotation)
            : Instantiate(spawnPrefabs[roomPlayer.GetComponent<PoleRoomPlayer>().SelectedCar], Vector3.zero, Quaternion.identity);
        gamePlayer.name = playerPrefab.name;
        gamePlayer.GetComponent<PlayerInfo>().Name = roomPlayer.GetComponent<PoleRoomPlayer>().Name;

        return gamePlayer;
    }

    #endregion

}
