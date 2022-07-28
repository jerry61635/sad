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

    }

    // Update is called once per frame
    void Update()
    {
        playerName.transform.LookAt(GameManager.Instance.Cam.transform);
        playerName.text = gameObject.name;
    }
}
