using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

[AddComponentMenu("")]
public class PoleRoomPlayer : NetworkRoomPlayer
{
    [SyncVar(hook = nameof(NameChanged))]
    public string Name;

    public override void OnStartClient()
    {
        if (LogFilter.Debug) Debug.LogFormat("OnStartClient {0}", SceneManager.GetActiveScene().path);

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

    #region SyncVar Hooks

    void NameChanged(string _, string newName)
    {
        //OnClientReady(newReadyState);
    }

    #endregion

    #region Commands

    [Command]
    public void CmdChangeName(string pName)
    {
        Name = pName;
    }

    #endregion
}
