using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 此腳本僅在魂之殿模式時才應該被掛載至玩家身上
 */

public class PlayerInSoul : MonoBehaviour
{
    /*
     * 魂之殿身分 以ID依序表示
     * 0愚者：入場即呈現離魂狀態。 
	 * 1女皇：回魂與被回魂時間減少５０％。	
     * 2正義：可清除場上所有玩家負面狀態，技能冷卻時間六十秒。
     * 3吊人：十秒內吸收所有愚者所受傷害與負面狀態，技能冷卻六十秒。
     */
    public float ID;    //玩家在於魂之殿的身分
    public bool soulOut;    //是否離魂
    public bool poison1;
    public bool poison2;
    void Start()
    {
        if (ID == 0) soulOut = true;
        else soulOut = false;
        poison1 = false;
        poison2 = false;
        GameManager.Instance.playerInSoulList.Add(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
