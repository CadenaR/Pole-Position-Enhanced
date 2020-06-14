using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

[AddComponentMenu("")]
public class PoleRoomPlayer : NetworkRoomPlayer
{
    [SyncVar]
    public string Name;

    [SyncVar]
    public int SelectedCar;

    public int maxLap;

    public static event Action<PoleRoomPlayer, string> OnMessage;

    public override void OnStartClient()
    {
        if (LogFilter.Debug) Debug.LogFormat("OnStartClient {0}", SceneManager.GetActiveScene().path);
        CmdChangeName("Player Joining...");
        OnMessage = null;
        OnMessage += FindObjectOfType<UIManager>().OnPlayerMessage;
        base.OnStartClient();
    }

    public override void OnClientEnterRoom()
    {
        if (LogFilter.Debug) Debug.LogFormat("OnClientEnterRoom {0}", SceneManager.GetActiveScene().path);
    }

    public override void OnClientExitRoom()
    {
        if (LogFilter.Debug) Debug.LogFormat("OnClientExitRoom {0}", SceneManager.GetActiveScene().path);
    }

    #region Commands

    [Command]
    public void CmdChangeName(string pName)
    {
        Name = pName;
    }

    [Command]
    public void CmdChangeCar(int car)
    {
        SelectedCar = car;
    }

    [Command]
    public void CmdSend(string message)
    {
        if (message.Trim() != "")
            RpcReceive(message.Trim());
    }

    [Command]
    public void CmdSetMaxLap(int lap)
    {
        RpcSetMaxLap(lap);
    }

    #endregion

    #region ClientRpc

    [ClientRpc]
    public void RpcReceive(string message)
    {
        OnMessage?.Invoke(this, message);
    }

    [ClientRpc]
    public void RpcSetMaxLap(int lap)
    {
        NetworkClient.connection.identity.GetComponent<PoleRoomPlayer>().maxLap = lap;
    }

    #endregion
}
