using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetchIP : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Local IP Address: " + IPManager.GetIP(ADDRESSFAM.IPv4));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
