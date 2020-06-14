using System;
using Mirror;
using UnityEngine;
using Random = System.Random;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class SetupPlayer : NetworkBehaviour
{
    [SyncVar] private int m_ID;
    [SyncVar] private string m_Name;

    private UIManager m_UIManager;
    private PoleNetworkManager m_NetworkManager;
    public PlayerController m_PlayerController;
    private PlayerInfo m_PlayerInfo;
    public PolePositionManager m_PolePositionManager;
    public bool raceStart;

    public GameObject carBody;
    public GameObject carWheelFR;
    public GameObject carWheelFL;
    public GameObject carWheelBR;
    public GameObject carWheelBL;

    #region Start & Stop Callbacks

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();
        m_ID = connectionToClient.connectionId;
    }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient()
    {
        base.OnStartClient();
        m_PlayerInfo.ID = m_ID;
        m_PlayerInfo.CurrentLap = 0;
        m_PolePositionManager.AddPlayer(m_PlayerInfo);
    }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer()
    {
    }

    #endregion

    private void Awake()
    {
        m_PlayerInfo = GetComponent<PlayerInfo>();
        m_PlayerController = GetComponent<PlayerController>();
        m_NetworkManager = FindObjectOfType<PoleNetworkManager>();
        m_PolePositionManager = FindObjectOfType<PolePositionManager>();
        m_UIManager = FindObjectOfType<UIManager>();        
    }

    // Start is called before the first frame update
    void Start()
    {
        raceStart = false;
        if (hasAuthority)
        {
            m_PlayerController.OnSpeedChangeEvent += OnSpeedChangeEventHandler;
            m_PlayerController.enabled = true;
            AppearCar();
            ConfigureCamera();
        }
    }

    void OnSpeedChangeEventHandler(float speed)
    {
        m_UIManager.UpdateSpeed((int) speed * 5); // 5 for visualization purpose (km/h)
    }

    void ConfigureCamera()
    {
        if (Camera.main != null) Camera.main.gameObject.GetComponent<CameraController>().m_Focus = this.gameObject;
    }

    void AppearCar()
    {
        this.GetComponentInParent<BoxCollider>().isTrigger = false;
        carBody.GetComponent<Renderer>().enabled = true;
        carWheelFR.GetComponent<Renderer>().enabled = true;
        carWheelFL.GetComponent<Renderer>().enabled = true;
        carWheelBR.GetComponent<Renderer>().enabled = true;
        carWheelBL.GetComponent<Renderer>().enabled = true;
    }

    #region Commands

    [Command]
    public void CmdStartRace()
    {        
        RpcSetRaceStart(true);
    }

    [Command]
    public void CmdEndClassification()
    {
        UnityEngine.Debug.Log("length 1: " + m_PolePositionManager.ordenSalida.Count);
        int pos = m_PolePositionManager.ordenSalida.IndexOf(this.GetComponentInParent<PlayerInfo>().ID);        
        FindObjectOfType<ParentCheck>().clasificacion = false;
        
        if(pos == -1)
        {
            pos = 3;            
        }

        RpcRestartPosition(pos);        
    }

    #endregion

    #region ClientRpc
    [ClientRpc]
    public void RpcRestartPosition(int pos){
        FindObjectOfType<UIManager>().startTime = NetworkTime.time;
        raceStart = false;
        this.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        this.GetComponent<Transform>().position = NetworkManager.startPositions[pos].position;
        this.GetComponent<Transform>().rotation = NetworkManager.startPositions[pos].rotation;
    }

    [ClientRpc]
    public void RpcSetRaceStart(bool b)
    {        
        raceStart = b;
        FindObjectOfType<PolePositionManager>().time.ResetTimer();
    }

    #endregion
}