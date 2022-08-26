using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoulMovement : MonoBehaviour
{
    //狀態宣告
    enum State
    {
        /* 此處為各狀態詳解
         * Sleep:   睡眠     玩家進入敵人偵測範圍內之前 AI不會作出任何動作
         * Idle:    待機     在範圍內有玩家時 進入攻擊前的攻擊間隔等待
         * Requim:  吸魂曲   使用後會將目標玩家掛載 "unknown.cs" 腳本
         * Poison1: 吮命毒蠱 施放前等待兩秒 離自身30單位內的玩家將每秒受到20傷害
         * Poison2: 沁滲腐水 
         * Run:     鴆酒殺機 加速並追逐目標玩家 並對目標使用Poison2 Melee 持續五秒
         * Melee:   天降洪水 遠程普通攻擊 等待兩秒
         * 
         * 此AI須辨別所有玩家的場地特有職業 
         */
        Sleep, Idle, Requiem, Poison1, Poison2, Run, Melee
    }

    //Global Reference
    NavMeshAgent agent;
    [SerializeField]
    State BossState;

    //AI數值設定
    public float stopDistance;  //與玩家保持的距離
    public float runSpeed;      //Run時Soul的移動速度
    public float detectRadius;
    public int targetPlayerIndex;
    public float idleDelay;
    public float requiemDelay;
    public float poison1Delay;
    public float poison2Delay;
    public float runTime;

    //Constant variable
    float randAttackMode; //
    float timer;                 //用於計時相關的變數
    float currentDistance;
    float[] playerDistance = new float[4];

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        BossState = State.Sleep;
        agent.speed = 0f;
        agent.stoppingDistance = stopDistance;
        timer = idleDelay;                              //起始Idle時間
    }

    // Update is called once per frame
    void Update()
    {        //Debug.Log(timer);
        //尋找最接近的玩家
        float minDistance = 100f;
        int j = 0;
        if (GameManager.Instance.playerList.Count != 0) {
            foreach (GameObject i in GameManager.Instance.playerList)
            {
                playerDistance[j] = Vector3.Distance(gameObject.transform.position, i.transform.position); //更新每個玩家與Soul的距離
                if (Vector3.Distance(gameObject.transform.position, i.transform.position) < minDistance)
                {
                    minDistance = Vector3.Distance(gameObject.transform.position, i.transform.position);
                    targetPlayerIndex = j;
                }
                j++;
            }
            agent.SetDestination(GameManager.Instance.playerList[targetPlayerIndex].transform.position);
        }

        switch (BossState)
        {
            case State.Sleep:
                if (currentDistance <= detectRadius)BossState = State.Idle;
                break;
            case State.Idle:
                if (timer >= 0) {  timer -= Time.deltaTime; }
                else
                {
                    randAttackMode = Random.Range(0, 20);
                    Debug.Log("Attack Mode: " + randAttackMode % 5);
                    switch (randAttackMode % 5)
                    {
                        case 0:
                            BossState = State.Requiem;
                            break;
                        case 1:
                            timer = poison1Delay;
                            BossState = State.Poison1;
                            break;
                        case 2:
                            timer = poison2Delay;
                            BossState = State.Poison2;
                            break;
                        case 3:
                            timer = runTime;
                            BossState = State.Run;
                            break;
                        case 4:
                            BossState = State.Melee;
                            break;
                    }
                }
                //Idle
                break;
            case State.Requiem:
                List<PlayerInSoul> targetList = new List<PlayerInSoul>(); //用於擷取尚未離魂的玩家
                for (int i = 0; i < GameManager.Instance.playerInSoulList.Count; i++)
                {
                    if (!GameManager.Instance.playerInSoulList[i].soulOut) targetList.Add(GameManager.Instance.playerInSoulList[i]);
                }

                if (targetList.Count != 0)
                {
                    int randTarget = Random.Range(0, targetList.Count);
                    targetList[randTarget].soulOut = true;
                }
                Debug.Log("Attack Done! Going back Idle.");
                timer = idleDelay;
                BossState = State.Idle;
                break;
            case State.Poison1:
                //毒蠱
                if (timer > 0) timer -= Time.deltaTime;
                else
                {
                    for (int i = 0; i < GameManager.Instance.playerList.Count; i++)
                    {
                        if (playerDistance[i] <= 30f) GameManager.Instance.playerInSoulList[i].poison1 = true;
                    }

                    Debug.Log("Attack Done! Going back Idle.");
                    timer = idleDelay;
                    BossState = State.Idle;
                }
                break;
            case State.Poison2:
                //腐水
                if (timer > 0) timer -= Time.deltaTime;
                else
                {
                    GameManager.Instance.playerInSoulList[targetPlayerIndex].poison2 = true;
                    Debug.Log("Attack Done! Going back Idle.");
                    timer = idleDelay;
                    BossState = State.Idle;
                }
                break;
            case State.Run:
                //唯一位移
                if(timer > 0)
                {
                    agent.speed = runSpeed;
                    timer -= Time.deltaTime;
                }
                else 
                {
                    agent.speed = 0f;
                    Debug.Log("Attack Done! Going back Idle.");
                    timer = idleDelay;
                    BossState = State.Idle;
                }
                break;
            case State.Melee:
                //普通攻擊
                Debug.Log("Attack Done! Going back Idle.");
                timer = idleDelay;
                BossState = State.Idle;
                break;
        }
    }
    
}

