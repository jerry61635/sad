using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : Player_Movement
{
    public static Player instance;

    //Player State
    
    public NetworkVariable<int> playerID = new NetworkVariable<int>(0);
    
    void Awake()
    {
        if(instance == null) instance = this;
    }


    void Start()
    {
        Debug.Log(playerID.Value);
        pause = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (IsLocalPlayer)
        {
            GameManager.Instance.FreeLook.m_LookAt = gameObject.transform;
            GameManager.Instance.FreeLook.m_Follow = gameObject.transform;
        }
        GameManager.Instance.playerData.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer && !UIFade.chatFocus)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            jump = Input.GetButton("Jump");
            dash = Input.GetKeyDown(KeyCode.LeftShift);
            interact = Input.GetKeyDown(KeyCode.E);
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                pause = !pause;
            }
            CharacterMove();
        }

        if (pause)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    /*
    [ServerRpc(RequireOwnership = false)]
    public void SyncPlayerServerRpc(string playerName)
    {
        SyncPlayerClientRpc(playerName);
    }

    [ClientRpc]
    public void SyncPlayerClientRpc(string playerName)
    {
        GameManager.Instance.playerList.Add(gameObject);
        GameManager.Instance.playerName.Add(playerName);
    }*/
}
