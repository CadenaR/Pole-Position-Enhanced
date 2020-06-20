using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class UIManager : NetworkBehaviour
{

    public double startTime;
    double t;
    public bool showGUI = true;

    private PoleNetworkManager m_NetworkManager;
    public PolePositionManager m_PositionManager;
    public SetupPlayer m_SetupPlayer;
    public Timer raceTimer;

    public string playerName { get; set; }

    public int carSelection { set { ChangeCar(value); } }

    public bool nextLap;
    public int CurrentLap;
    public int MaxLap;


    [Header("Main Menu")] [SerializeField] private GameObject mainMenu;
    [SerializeField] private Button buttonHost;
    [SerializeField] private Button buttonClient;
    [SerializeField] private Button buttonServer;
    [SerializeField] private InputField inputFieldIP;

    [Header("Room Menu")]
    public TMP_Text[] playerNameTexts = new TMP_Text[4];
    public TMP_Text[] playerReadyTexts = new TMP_Text[4];
    [SerializeField] private Button buttonReturn;
    public GameObject lapsObject;
    public InputField inputLaps;
    //For Chat
    public InputField chatMessage;
    public Text chatHistory;
    public Scrollbar scrollbar;
    public Toggle classifLapToggle;

    [Header("In-Game HUD")]
    [SerializeField]
    private GameObject inGameHUD;
    [SerializeField] private Text textTime;
    [SerializeField] private Text textSpeed;
    [SerializeField] public Text textLaps;
    [SerializeField] public Text textPosition;
    [SerializeField] private Text Semaphore;
    [SerializeField] public Text EndPlayers;
    [SerializeField] public Text EndTimes;
    public GameObject loadingPanel;
    public GameObject pauseMenu;
    public Scrollbar timesScroll;
    public Text timesText;

    public bool ready = false;

    private void Awake()
    {
        m_NetworkManager = FindObjectOfType<PoleNetworkManager>();
        raceTimer = new Timer();
    }

    private void Start()
    {

        buttonHost.onClick.AddListener(() => StartHost());
        buttonClient.onClick.AddListener(() => StartClient());
        buttonServer.onClick.AddListener(() => StartServer());

        if (SceneManager.GetActiveScene().name == "RoomScene")
        {
            buttonReturn.onClick.AddListener(() => GoBack());
        }
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            startTime = NetworkTime.time;
            textPosition.transform.parent.gameObject.SetActive(false);
            textTime.transform.parent.gameObject.SetActive(false);
            nextLap = false;
            timesText.text = "";
        }
    }

    public void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name == "GameScene" && FindObjectOfType<CameraController>() != null)
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pauseMenu.SetActive(!pauseMenu.active);
            }

            if (m_PositionManager == null)
            {
                m_PositionManager = FindObjectOfType<PolePositionManager>();
                
            }
            if(m_SetupPlayer == null)
            {
                foreach(SetupPlayer p in FindObjectsOfType<SetupPlayer>())
                {
                    if (p.hasAuthority)
                    {
                        m_SetupPlayer = p;
                    }
                }
                return;
            }

            UpdateGameGUI(); 
        }
        if (SceneManager.GetActiveScene().name == "RoomScene")
        {
            UpdateRoomGUI();
        }    
    }

    public void ActivateMainMenu()
    {
        mainMenu.SetActive(true);
        inGameHUD.SetActive(false);
    }

    public void ActivateInGameHUD()
    {
        mainMenu.SetActive(false);
        inGameHUD.SetActive(true);
    }

    public void StartHost()
    {
        m_NetworkManager.StartHost();
        ActivateInGameHUD();
    }

    public void StartClient()
    {
        if (inputFieldIP.text.Trim() == "")
        {
            m_NetworkManager.StartClient();
        }
        else
        {
            m_NetworkManager.StartClient(new Uri(inputFieldIP.text.Trim()));
        }
        ActivateInGameHUD();
    }

    public void StartServer()
    {
        m_NetworkManager.StartServer();
        ActivateInGameHUD();
    }

    public void GoBack()
    {
        if (NetworkClient.connection.identity.isServer)
        {
            m_NetworkManager.StopHost();
        }
        else
        {
            m_NetworkManager.StopClient();
        }
    }

    #region Room
    public void ChangeName()
    {
        // Calls CmdChangeName only on the local player
        foreach (NetworkRoomPlayer item in m_NetworkManager.roomSlots)
        {
            if (item == null)
                continue;

            if (item.hasAuthority)
            {
                item.GetComponentInParent<PoleRoomPlayer>().CmdChangeName(playerName);

            }

        }
    }

    public void ChangeCar(int car)
    {
        // Calls CmdChangeCar only on the local player
        foreach (NetworkRoomPlayer item in m_NetworkManager.roomSlots)
        {
            if (item == null)
                continue;

            if (item.hasAuthority)
                item.GetComponentInParent<PoleRoomPlayer>().CmdChangeCar(car);
        }
    }

    public void PlayerReady()
    {
        foreach (NetworkRoomPlayer item in m_NetworkManager.roomSlots)
        {
            if (item == null)
                continue;

            if (item.hasAuthority)
            {
                item.CmdChangeReadyState(!item.readyToBegin);
            }
        }
    }

    //Updates player's names & statuses
    private void UpdateRoomGUI()
    {
        foreach (TMP_Text field in playerNameTexts)
        {
            field.text = "Waiting for Player...";
        }

        foreach (TMP_Text field in playerReadyTexts)
        {
            field.text = "";
        }

        for (int i = 0; i < m_NetworkManager.roomSlots.Count; i++)
        {

            if (m_NetworkManager.roomSlots[i] == null)
            {
                m_NetworkManager.roomSlots.RemoveAt(i);
                continue;
            }

            if (m_NetworkManager.roomSlots[i].GetComponentInParent<PoleRoomPlayer>().Name == "")
            {
                playerNameTexts[i].text = "Player " + (i + 1);
            }
            else
            {
                playerNameTexts[i].text = m_NetworkManager.roomSlots[i].GetComponentInParent<PoleRoomPlayer>().Name;
            }

            if (m_NetworkManager.roomSlots[i].readyToBegin)
            {
                playerReadyTexts[i].text = "<color=green>Ready</color>";
            }
            else
            {
                playerReadyTexts[i].text = "<color=red>Not Ready</color>";
            }
        }
    }

    //This is only for room owner
    //Updates its laps with the inputted number
    public void SetLapsOnPlayer()
    {
        if (inputLaps.text.Trim() == "")
        {
            m_NetworkManager.roomSlots[0].GetComponent<PoleRoomPlayer>().CmdSetMaxLap(3);
            return;
        }

        int num = Int16.Parse(inputLaps.text.Trim());
        if (num < 1)
        {
            m_NetworkManager.roomSlots[0].GetComponent<PoleRoomPlayer>().CmdSetMaxLap(1);
            return;
        }
        m_NetworkManager.roomSlots[0].GetComponent<PoleRoomPlayer>().CmdSetMaxLap(num);
    }

    public void SetClassifLap()
    {
        m_NetworkManager.roomSlots[0].GetComponent<PoleRoomPlayer>().CmdChangeClassifLap();
    }

    //Chat
    public void OnPlayerMessage(PoleRoomPlayer player, string message)
    {
        string prettyMessage = player.isLocalPlayer ?
            $"<color=red>You: </color> {message}" :
            $"<color=blue>{player.Name}: </color> {message}";
        AppendMessage(prettyMessage);

        Debug.Log(message);
    }

    public void OnSend()
    {
        if (!Input.GetKeyDown(KeyCode.Return)) return;
        if (chatMessage.text.Trim() == "")
            return;

        // get our player
        PoleRoomPlayer player = NetworkClient.connection.identity.GetComponent<PoleRoomPlayer>();

        // send a message
        player.CmdSend(chatMessage.text.Trim());

        chatMessage.text = "";
    }

    internal void AppendMessage(string message)
    {
        StartCoroutine(AppendAndScroll(message));
    }

    IEnumerator AppendAndScroll(string message)
    {
        chatHistory.text += message + "\n";

        // it takes 2 frames for the UI to update ?!?!
        yield return null;
        yield return null;

        // slam the scrollbar down
        scrollbar.value = 0;
    }

    public void AppendLapTime(string lapTime)
    {
        StartCoroutine(AppendAndScrollLaps(lapTime));
    }

    IEnumerator AppendAndScrollLaps(string message)
    {
        timesText.text += message + "\n";

        // it takes 2 frames for the UI to update ?!?!
        yield return null;
        yield return null;

        // slam the scrollbar down
        timesScroll.value = -3;
    }

    #endregion

    #region GameRoom

    public void UpdateGameGUI()
    {
        if (m_SetupPlayer.raceStart)
        {
            if (!m_SetupPlayer.classifLap)
            {
                if(!textTime.transform.parent.gameObject.active)
                    textTime.transform.parent.gameObject.SetActive(true);

                if (!textPosition.transform.parent.gameObject.active)
                {
                    if ((int)raceTimer.t >= 6)
                        textPosition.transform.parent.gameObject.SetActive(true);
                }
                
                textTime.text = raceTimer.timerText;
                UpdateLaps();
            }
            if (Semaphore.text == "go!" && (int)raceTimer.t >= 1)
            {
                Semaphore.text = "";
            }

            raceTimer.UpdateTimer();
        }
        else
        {
            t = (NetworkTime.time - startTime) % 60;
            if (t >= 2)
            {
                Semaphore.text = "" + (5 - (int)t);
                if (t >= 5)
                {
                    Semaphore.text = "go!";
                    m_SetupPlayer.CmdStartRace();
                }
            }
        }

    }

    public void UpdateLaps()
    {
        if (NetworkClient.connection == null) return;
        CurrentLap = NetworkClient.connection.identity.GetComponent<PlayerInfo>().lap;
        MaxLap = NetworkClient.connection.identity.GetComponent<PlayerInfo>().maxLap;
        if (CurrentLap <= MaxLap)
            textLaps.text = "Lap " + CurrentLap + "/" + MaxLap;
    }

    public void UpdateSpeed(int speed)
    {
        textSpeed.text = "Speed " + speed + " Km/h";
    }

    public void UpdatePositions(string myRaceOrder)
    {
        textPosition.text = myRaceOrder;
    }

    #endregion

    #region End

    public void ExitGame()
    {
        if (NetworkClient.connection.identity.GetComponent<SetupPlayer>().isClientOnly)
        {
            m_NetworkManager.StopClient();
        }
        else 
        {
            m_NetworkManager.StopHost();
        }
    }
    
    #endregion
}

