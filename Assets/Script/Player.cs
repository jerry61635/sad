using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : Player_Movement
{
    public static Player instance;

    //Player State
    public string name_p;
    
    void Awake()
    {
        if(instance == null) instance = this;
    }


    void Start()
    {
        if(IsLocalPlayer)
        {
            if(IsServer) SyncPlayerClientRpc(ConnectHUD.PlayerName);
            else SyncPlayerServerRpc(ConnectHUD.PlayerName);
        }
        pause = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (IsLocalPlayer)
        {
            GameManager.Instance.FreeLook.m_LookAt = gameObject.transform;
            GameManager.Instance.FreeLook.m_Follow = gameObject.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        name_p = gameObject.name;
        if (IsClient && !UIFade.chatFocus)
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
        CharacterMove();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SyncPlayerServerRpc(string playerName)
    {
        if (!IsServer) return;

        SyncPlayerClientRpc(playerName);
    }

    [ClientRpc]
    public void SyncPlayerClientRpc(string playerName)
    {
        GameManager.Instance.playerList.Add(gameObject);
        GameManager.Instance.playerName.Add(playerName);
    }
}
