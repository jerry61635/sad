using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerHUD : NetworkBehaviour
{
    public Text playerName;
    // Start is called before the first frame update
    void Start()
    {
        if(IsOwner) HUDServerRpc(ConnectHUD.PlayerName);
    }

    // Update is called once per frame
    void Update()
    {
        playerName.transform.LookAt(GameManager.Instance.Cam.transform);

        if (IsServer && IsOwner) HUDClientRpc(ConnectHUD.PlayerName);
    }

    [ServerRpc(RequireOwnership = false )]
    void HUDServerRpc(string Pname)
    {
        HUDClientRpc(Pname);
        name = Pname;
    }

    [ClientRpc]
    void HUDClientRpc(string Pname)
    {
        playerName.text = Pname;
    }
}
