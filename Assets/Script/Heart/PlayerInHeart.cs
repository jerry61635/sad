using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInHeart : MonoBehaviour
{
    public int ID;
    public bool[] getHeart = new bool[4];
    public bool stun;
    public bool parasite;
    float timer;
    
    void Start()
    {
        GameManager.Instance.playerInHeartList.Add(this);
        for(int i = 0; i < 4; i++) { getHeart[i] = false; }
        stun = false;
        parasite = false;
        getHeart[2] = true;
        timer = 0;
    }


    void Update()
    {
        if (stun && timer <= 0) timer = 5;
        if (timer > 0) 
        { 
            //Debug.Log("Stun time remain: " + timer);
            Player.instance.stun = true;
            timer -= Time.deltaTime;
        }
        else Player.instance.stun = false;
        if (parasite) Player.instance.speedDelta = 0.5f;
        else Player.instance.speedDelta = 1f;
    }
}
