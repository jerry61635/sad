using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetDebug : NetworkBehaviour
{
    public NetworkObject PlayerNetwokObject;
    public bool server;
    public bool owner;
    public bool client;
    public bool local;
    public bool host;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer) server = true;
        else server = false;

        if (IsOwner) owner = true;
        else owner = false;

        if (IsClient) client = true;
        else client = false;

        if (IsLocalPlayer) local = true;
        else local = false;

        if (IsHost) host = true;
        else host = false;

    }
}
