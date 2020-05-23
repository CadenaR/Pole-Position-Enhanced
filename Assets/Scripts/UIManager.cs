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
    public bool showGUI = true;

    private PoleNetworkManager m_NetworkManager;

    public string playerName { get; set; }

    public int carSelection { set { ChangeCar(value); } }

    [Header("Main Menu")] [SerializeField] private GameObject mainMenu;
    [SerializeField] private Button buttonHost;
    [SerializeField] private Button buttonClient;
    [SerializeField] private Button buttonServer;
    [SerializeField] private InputField inputFieldIP;

    [Header("Room Menu")]
    public TMP_Text[] playerNameTexts = new TMP_Text[4];
    public TMP_Text[] playerReadyTexts = new TMP_Text[4];
    //For Chat
    public InputField chatMessage;
    public Text chatHistory;
    public Scrollbar scrollbar;

    [Header("In-Game HUD")] [SerializeField]
    private GameObject inGameHUD;

    [SerializeField] private Text textSpeed;
    [SerializeField] private Text textLaps;
    [SerializeField] private Text textPosition;

    private void Awake()
    {
        m_NetworkManager = FindObjectOfType<PoleNetworkManager>();
        
    }

    private void Start()
    {
        buttonHost.onClick.AddListener(() => StartHost());
        buttonClient.onClick.AddListener(() => StartClient());
        buttonServer.onClick.AddListener(() => StartServer());

        if (SceneManager.GetActiveScene().name == "RoomScene")
            PoleRoomPlayer.OnMessage += OnPlayerMessage;
    }

    public void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name != "RoomScene")
            return;

        UpdateGUI();
    }

    public void UpdateSpeed(int speed)
    {
        textSpeed.text = "Speed " + speed + " Km/h";
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
        m_NetworkManager.StartClient();
        m_NetworkManager.networkAddress = inputFieldIP.text;
        ActivateInGameHUD();
    }

    public void StartServer()
    {
        m_NetworkManager.StartServer();
        ActivateInGameHUD();
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

    private void UpdateGUI()
    {
        foreach (TMP_Text field in playerNameTexts)
        {
            field.text = "Waiting for Player...";
        }

        foreach (TMP_Text field in playerReadyTexts)
        {
            field.text = "";
        }

        //foreach (NetworkRoomPlayer item in m_NetworkManager.roomSlots)
        for(int i = 0; i < m_NetworkManager.roomSlots.Count; i++)
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

    //Chat
    private void OnPlayerMessage(PoleRoomPlayer player, string message)
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

    #endregion

}