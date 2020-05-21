using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreatePlayer);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        // tell the server to create a player with this name
        conn.Send(new CreatePlayerMessage { name = PlayerName });
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //Overriding with empty project so OnCreatePlayer create the player assigning the entered name.
        //This avoids from creating two playerClones for the same client
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
    }
}
