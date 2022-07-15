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


    private void Update()
    {
        for(int i = 0; i < playerList.Count; i++)
        {
            if(playerList[i] == null)
            {
                playerList.RemoveAt(i);
            }
        }
    }

    [ClientRpc]
    public void SyncPlayerClientRpc()
    {

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
