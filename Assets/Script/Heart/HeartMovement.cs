using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HeartMovement : MonoBehaviour
{
    enum State
    {
        /*  Heart 行為模式
         *  
         *  颶霾捲襲：前搖兩秒後煽動骨翅，所有在場玩家以敵方為中心，向內造成一１．５公尺／秒之速度持續５秒，此技能有２５秒冷卻時間。
         *  
         *  魘魂恩寵：當前持有代罪心臟之玩家，立即囚固於原地５秒，技能每３０秒觸發一次。
         *  
         *  冬蟲夏草：以當前所有玩家各自所在地為圓心，於５公尺圓形範圍內播種冬蟲夏草，一秒後若玩家未離開此範圍，玩家移動速度減緩５０％
         *  所受傷害提升５０％，並且每秒受毒菌傷害１０點生命持續５秒。此技能有４０秒冷卻時間。
         *  
         *  怨附鹿掌（普通攻擊）：蓄力兩秒後瘋狂砸及地面以產生波動，以敵人為圓心，向前方９０度角範圍內攻擊
         *  長至５０公尺。此技能有１５秒冷卻時間。
         *  
         *  風行草偃：前搖兩秒後煽動骨翅，所有在場玩家以敵方為中心，向內造成一１公尺／秒之速度持續３秒
         *  並衝撞向離自身最近之玩家造成１００傷害。此技能有２０秒冷卻時間。
         */
        Sleep, Idle, Pull, Stun, Parasite, Melee, Rush
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

    [Header("Idle Settings")]
    public float idleDelay;

    [Header("颶霾捲襲")]
    public float pullingForce;
    public float pullingTime;

    [Header("魘魂恩寵")]
    public float stunTime;

    [Header("冬蟲夏草")]
    public float parasiteDelay;
    public float parasiteDistance;
    public float parasiteTime;

    [Header("怨附鹿掌")]
    public float meleeDelay;

    [Header("風行草偃")]
    public float rushDelay;
    public float rushPullingTime;
    public float rushPullingForce;
    public float rushSpeed;
    public float rushDistance;

    //Constant variable
    float randAttackMode;        //決定攻擊
    float timer;                 //用於計時相關的變數
    int stage;                   //用於某些狀態有多次使用計時器時使用
    float[] playerDistance = new float[4]; //儲存每個玩家與Heart的距離

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        BossState = State.Sleep;
        timer = idleDelay;
    }

    // Update is called once per frame
    void Update()
    {
        switch (BossState)
        {
            case State.Sleep:
                if(GameManager.Instance.playerList.Count != 0)
                {
                    foreach(GameObject i in GameManager.Instance.playerList)
                    {
                        if (Vector3.Distance(gameObject.transform.position, i.transform.position) <= detectRadius)
                            BossState = State.Idle;
                    }
                }
                break;

            case State.Idle:
                int j = 0;                      //Index
                float minDistance = 1000f;      //用於儲存最短距離的變數
                //尋找最接近的玩家 並且儲存玩家與Heart的距離
                foreach (GameObject i in GameManager.Instance.playerList)
                {
                    playerDistance[j] = Vector3.Distance(gameObject.transform.position, i.transform.position);
                    if (minDistance > playerDistance[j])
                    {
                        minDistance = playerDistance[j];
                        targetPlayerIndex = j;
                    }
                    j++;
                    
                }
                //Idle狀態停留的時間
                if (timer > 0) timer -= Time.deltaTime;
                //攻擊模式選擇
                else
                {
                    randAttackMode = Random.Range(0, 20);
                    Debug.Log("Attack Mode: " + randAttackMode % 5);
                    switch(randAttackMode % 5)
                    {
                        case 0:
                            BossState = State.Pull;
                            timer = pullingTime;
                            break;
                        case 1:
                            timer = stunTime;
                            BossState = State.Stun;
                            break;
                        case 2:
                            timer = parasiteDelay;
                            BossState = State.Parasite;
                            break;
                        case 3:
                            timer = meleeDelay;
                            BossState = State.Melee;
                            break;
                        case 4:
                            stage = 0;
                            timer = rushDelay;
                            BossState = State.Rush;
                            break;
                    }
                }
                break;
            case State.Pull:
                //技能持續時間
                if (timer > 0)
                {
                    foreach (Player i in GameManager.Instance.playerData)
                    {
                        i.controller.Move((gameObject.transform.position - i.transform.position).normalized * Time.deltaTime * pullingForce);
                    }
                    timer -= Time.deltaTime;
                }
                //返回Idle
                else
                {
                    timer = idleDelay;
                    BossState = State.Idle;
                }
                break;
            case State.Stun:
                //暈眩玩家
                if (timer > 0)
                {
                    foreach (PlayerInHeart i in GameManager.Instance.playerInHeartList)
                    {
                        for (int z = 0; z < 4; z++)
                        {
                            if (i.getHeart[z]) i.stun = true;
                        }
                    }
                    timer -= Time.deltaTime;
                }
                //返回Idle
                else
                {
                    timer = idleDelay;
                    BossState = State.Idle;
                }
                break;

            case State.Parasite:
                //
                if (timer > 0) 
                { 
                    timer -= Time.deltaTime;
                    if(stage == 1)
                    for (int i = 0; i < GameManager.Instance.playerInHeartList.Count; i++)
                    {
                        if (playerDistance[i] < parasiteDistance)
                        {
                            GameManager.Instance.playerInHeartList[i].parasite = true;
                        }
                    }
                }
                else
                {
                    stage++;
                    if (stage == 0) { timer = parasiteTime; }
                    else
                    {
                        for (int i = 0; i < GameManager.Instance.playerInHeartList.Count; i++)
                            GameManager.Instance.playerInHeartList[i].parasite = false;
                        timer = idleDelay;
                        BossState = State.Idle;
                    }
                }
                break;
            case State.Melee:
                
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    timer = idleDelay;
                    BossState = State.Idle;
                }
                break;
            case State.Rush:
                //Stage 0:前搖準備(Movement Delay)
                //Stage 1:技能持續時間
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                    if (stage == 1)
                    {
                        foreach (Player i in GameManager.Instance.playerData) //取得玩家資訊(Transform, Controller)
                        {
                            i.controller.Move((gameObject.transform.position - i.transform.position).normalized * Time.deltaTime * rushPullingForce);
                        }
                    }
                    else
                    {
                        agent.speed = rushSpeed;
                        agent.stoppingDistance = rushDistance;
                    }
                }
                else
                {
                    stage++;
                    //前搖讀秒完成
                    if (stage == 0) { timer = rushPullingTime; }
                    //技能施放完成
                    else if(stage == 2)
                    {
                        agent.speed = runSpeed;
                        agent.stoppingDistance = stopDistance;
                        timer = idleDelay;
                        BossState = State.Idle;
                    }
                }
                break;
        }
    }
}
