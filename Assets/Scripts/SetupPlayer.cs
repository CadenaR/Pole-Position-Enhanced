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

    [SyncVar]
    public bool classifLap;

    [SyncVar]
    public bool raceStart;

    private UIManager m_UIManager;
    private PoleNetworkManager m_NetworkManager;
    public PlayerController m_PlayerController;
    private PlayerInfo m_PlayerInfo;
    public PolePositionManager m_PolePositionManager;


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
        m_PolePositionManager.AddPlayer(m_PlayerInfo);
    }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        raceStart = false;
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
        if (!classifLap)
        {
            AppearCar();
        }
        else
        {
            if (isClientOnly)
            {
                if (!hasAuthority)
                    this.GetComponent<Collider>().enabled = false;
            }
        }

        if (hasAuthority)
        {
            FindObjectOfType<ParentCheck>().RestartCheckpoints();
            m_PlayerController.OnSpeedChangeEvent += OnSpeedChangeEventHandler;
            m_PlayerController.enabled = true;
            AppearCar();
            ConfigureCamera();

            if (classifLap)
            {
                FindObjectOfType<UIManager>().textLaps.text = "Classif.";
            }
            else
            {
                FindObjectOfType<UIManager>().UpdateLaps();
            }
        }
    }

    private void FixedUpdate()
    {
        if (this.GetComponent<Transform>().position.y > 3 || this.GetComponent<Transform>().position.y < 0.3)
        {
            this.GetComponent<Transform>().position = new Vector3(GetComponent<Transform>().position.x, 0.5f, GetComponent<Transform>().position.z);
        }
        if(raceStart){            
            FindObjectOfType<PolePositionManager>().time.UpdateTimer();
        }
    }

    void OnSpeedChangeEventHandler(float speed)
    {
        m_UIManager.UpdateSpeed((int)speed * 5); // 5 for visualization purpose (km/h)
    }

    void ConfigureCamera()
    {
        if (Camera.main != null) Camera.main.gameObject.GetComponent<CameraController>().m_Focus = this.gameObject;
    }

    public void AppearCar()
    {
        carBody.GetComponent<Renderer>().enabled = true;
        carWheelFR.GetComponent<Renderer>().enabled = true;
        carWheelFL.GetComponent<Renderer>().enabled = true;
        carWheelBR.GetComponent<Renderer>().enabled = true;
        carWheelBL.GetComponent<Renderer>().enabled = true;
        this.GetComponent<Collider>().enabled = true;
    }

    void OnCollisionEnter(Collision collision)
    {        
        if (classifLap && (collision.gameObject.GetComponent<WheelCollider>() != null || collision.gameObject.tag == "Player"))
        {     
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), this.GetComponent<Collider>());
        }
    }

    #region Commands

    [Command]
    public void CmdStartRace()
    {
        raceStart = true;        
        FindObjectOfType<UIManager>().raceTimer.ResetTimer();
        RpcRaceStart();
    }

    [Command]
    public void CmdEndClassification()
    {        
        RpcRestartPosition();

    }

    [Command]
    public void CmdSetClassifLap(bool classif)
    {
        classifLap = classif;
    }

    #endregion

    #region ClientRpc
    [ClientRpc]
    public void RpcRestartPosition()
    {
        if (hasAuthority)
        {
            FindObjectOfType<UIManager>().startTime = NetworkTime.time;
            FindObjectOfType<ParentCheck>().RestartCheckpoints();
            FindObjectOfType<UIManager>().UpdateLaps();
        }
    }

    [ClientRpc]
    void RpcRaceStart()
    {
        FindObjectOfType<UIManager>().raceTimer.ResetTimer();
        FindObjectOfType<PolePositionManager>().time.ResetTimer();
    }

    [Server]
    public void SetRaceStart(bool b)
    {
        raceStart = b;
        FindObjectOfType<PolePositionManager>().time.ResetTimer();
    }

    [ClientRpc]
    public void RpcUpdatePositions(string myRaceOrder)
    {
        FindObjectOfType<UIManager>().UpdatePositions(myRaceOrder);
    }

    [ClientRpc]
    public void RpcUpdateClientEnd(string pl, string tim){
        m_UIManager.EndPlayers.text = pl;
        m_UIManager.EndTimes.text = tim;
    }

    [ClientRpc]
    public void RpcAppear()
    {
        AppearCar();
    }

    #endregion
}