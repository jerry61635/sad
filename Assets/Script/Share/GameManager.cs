using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    //public event System.Action<Player_Movement> OnLocalPlayerJoin;
    //private GameObject gameObject;

    public List<GameObject> playerList = new List<GameObject>();
    public List<PlayerInSoul> playerInSoulList = new List<PlayerInSoul>();
    public List<string> playerName = new List<string>();

    public Camera Cam;
    public Cinemachine.CinemachineFreeLook FreeLook;
    public GameObject chatPanel, textObject;
    public InputField chatBox;
    public CanvasGroup chatCanvasGroup;

    private static GameManager m_Instance;
    public static GameManager Instance;

    void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    void Start()
    {
        Debug.developerConsoleVisible = true;
    }

    private void Update()
    {
        //if(IsServer) SyncPlayerClientRpc();
        //else SyncPlayerServerRpc();

        for(int i = 0; i < playerList.Count; i++)
        {
            if(playerList.Count == 0) break;
            
            if(playerList[i] == null)
            {
                playerList.RemoveAt(i);
                playerName.RemoveAt(i);
            }
        }

        for(int i = 0; i < playerList.Count; i++)
        {
            playerList[i].name = playerName[i];
        }

        if(playerInSoulList.Count != 0)
        {
            for(int i = 0; i < playerInSoulList.Count; i++)
            {
                playerInSoulList[i].ID = i;
            }
        }

        //Debug.Log(playerList);
    }


    /*
        private Player_Movement m_localPlayer;
        public Player_Movement localPlayer{
            get
            {
                return m_localPlayer;
            }
            set
            {
                m_localPlayer = value;
                if(OnLocalPlayerJoin != null){
                    OnLocalPlayerJoin();
                }
            }
        }*/
}
