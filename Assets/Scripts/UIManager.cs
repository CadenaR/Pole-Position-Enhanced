using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : NetworkBehaviour
{
    public bool showGUI = true;

    private PoleNetworkManager m_NetworkManager;

    public string playerName {set { ChangeName(value); }}

    public int carSelection { set { ChangeCar(value); } }

    [Header("Main Menu")] [SerializeField] private GameObject mainMenu;
    [SerializeField] private Button buttonHost;
    [SerializeField] private Button buttonClient;
    [SerializeField] private Button buttonServer;
    [SerializeField] private InputField inputFieldIP;

    [Header("Room Menu")]
    [SerializeField] public TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] public TMP_Text[] playerReadyTexts = new TMP_Text[4];


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

    
    public void ChangeName(string pName)
    {
        // Calls CmdChangeName only on the local player
        foreach (NetworkRoomPlayer item in m_NetworkManager.roomSlots)
        {
            if (item.hasAuthority)
                item.GetComponentInParent<PoleRoomPlayer>().CmdChangeName(pName);

            m_NetworkManager.GUIUpdate();
        }
    }

    public void ChangeCar(int car)
    {
        // Calls CmdChangeCar only on the local player
        foreach (NetworkRoomPlayer item in m_NetworkManager.roomSlots)
        {
            if (item.hasAuthority)
                item.GetComponentInParent<PoleRoomPlayer>().CmdChangeCar(car);
        }
    }

    public void PlayerReady()
    {
        foreach (NetworkRoomPlayer item in m_NetworkManager.roomSlots)
        {
            if (item.hasAuthority)
                item.CmdChangeReadyState(!item.readyToBegin);

            m_NetworkManager.GUIUpdate();
        }
    }


}