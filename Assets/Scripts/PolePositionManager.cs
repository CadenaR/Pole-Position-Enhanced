using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Mirror;
using UnityEngine;
using System.Threading;

public class PolePositionManager : NetworkBehaviour
{
    //public class SyncListOrder : SyncList<int> { }
    public SemaphoreSlim playerArraySemaphore = new SemaphoreSlim(1);
    public SemaphoreSlim positionOrder = new SemaphoreSlim(1);
    public int numPlayers;
    public PoleNetworkManager networkManager;    
    public Timer time = new Timer();
    public List<int> raceOrder = new List<int>();
    //public SyncListOrder ordenSalida = new SyncListOrder();
    private readonly List<PlayerInfo> m_Players = new List<PlayerInfo>(4);
    private List<PlayerInfo> m_Players_Clone = new List<PlayerInfo>(4);
    private CircuitController m_CircuitController;
    private GameObject[] m_DebuggingSpheres;
    public bool startRace = false;

    private void Awake()
    {
        if (networkManager == null) networkManager = FindObjectOfType<PoleNetworkManager>();
        if (m_CircuitController == null) m_CircuitController = FindObjectOfType<CircuitController>();

        m_DebuggingSpheres = new GameObject[networkManager.maxConnections];
        for (int i = 0; i < networkManager.maxConnections; ++i)
        {
            m_DebuggingSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m_DebuggingSpheres[i].GetComponent<SphereCollider>().enabled = false;
        }
    }  

    private void Update()
    {   
        if (m_Players.Count == 0){
            return;
        }
        UpdateRaceProgress();
        
    }

    public void AddPlayer(PlayerInfo player)
    {
        playerArraySemaphore.Wait();
        m_Players.Add(player);
        m_Players_Clone.Add(player);
        playerArraySemaphore.Release();
    }

    private class PlayerInfoComparer : Comparer<PlayerInfo>
    {
        float[] m_ArcLengths;

        public PlayerInfoComparer(float[] arcLengths)
        {
            m_ArcLengths = arcLengths;
        }

        public override int Compare(PlayerInfo x, PlayerInfo y)
        {
            if (this.m_ArcLengths[x.ID] < m_ArcLengths[y.ID])
                return 1;
            else return -1;
        }
    }

    public void UpdateRaceProgress()
    {
        for (int i = 0; i < m_Players_Clone.Count; ++i)
        {
            if(this.m_Players_Clone[i] == null)
            {
                this.m_Players_Clone.RemoveAt(i);
                return;
            }
        }
        // Update car arc-lengths
        float[] arcLengths = new float[m_Players.Count];

        playerArraySemaphore.Wait();
        for (int i = 0; i < m_Players.Count; ++i)
        {
            if(this.m_Players[i] == null)
            {
                this.m_Players.RemoveAt(i);
                this.m_DebuggingSpheres[i].transform.position = new Vector3(0, 0, 0);
                playerArraySemaphore.Release();
                return;
            }
            arcLengths[i] = ComputeCarArcLength(i);
        }

        m_Players_Clone.Sort(new PlayerInfoComparer(arcLengths));
        playerArraySemaphore.Release();
        //Debug.Log("Jugadores " + m_Players.Count);
        string myRaceOrder = "";
        int playerPlace = 1;
        foreach (var _player in m_Players_Clone)
        {
            myRaceOrder += playerPlace + "° " +  _player.Name + "\n ";
            playerPlace++;
        }

        FindObjectOfType<UIManager>().UpdatePositions(myRaceOrder);
        //Debug.Log("El orden de carrera es: " + myRaceOrder);
    }

    float ComputeCarArcLength(int ID)
    {
        // Compute the projection of the car position to the closest circuit 
        // path segment and accumulate the arc-length along of the car along
        // the circuit.
        Vector3 carPos = this.m_Players[ID].transform.position;

        int segIdx;
        float carDist;
        Vector3 carProj;

        float minArcL =
            this.m_CircuitController.ComputeClosestPointArcLength(carPos, out segIdx, out carProj, out carDist);

        this.m_DebuggingSpheres[ID].transform.position = carProj;

        if (this.m_Players[ID].lap == 1)
        {
            minArcL += m_CircuitController.CircuitLength;
        }
        else
        {
            minArcL += m_CircuitController.CircuitLength *
                    (m_Players[ID].lap);
        }

        return minArcL;
    }  

    public void AddPlayerPosition(int Id){
        positionOrder.Wait();
        raceOrder.Add(Id);
        positionOrder.Release();
    } 
}