using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShuraMovement : MonoBehaviour
{
    public GameObject aModeAttack;
    public GameObject bModeAttack;
    public GameObject appModeAttack;
    public GameObject bppModeAttack;

    //常數
    float timer;
    float delayTime;
    float IdleTime;
    float currentDistance;
    float randAttackMode;
    float randCoin;
    public GameObject targetPlayer;

    public List<GameObject> triggerPrefab = new List<GameObject>();

    public float phase = 0;  //血量60%以上第一階段 30%以上第二階段 30%以下最終階段
    public float health = 100f;
    float maxHealth;
    [SerializeField]
    State BossState = State.Sleep;

    public float aModeDelay;    //A狀態的攻擊前延遲
    public float aSpeed;        //A攻擊的上升速度
    public float bModeDelay;    //B狀態的攻擊前延遲
    public float bSpeed;        //B攻擊的子彈飛行速度
    public float bTimer;        //B攻擊的子彈飛行時間
    public float cModeDelay;
    public float cTimer;
    public float rushSpeed;     //C狀態的衝刺速度
    public float rushRadius;
    public float stage;         //用以辨識階段三的運行階段

    //Agent reference
    public NavMeshAgent agent;
    public float detectRadius;  //修羅甦醒的偵測距離
    public float walkSpeed;     //修羅的行走速度
    public float stopDistance;  //修羅與玩家間的預設距離

    void Start()
    {
        IdleTime = 2f;
        agent.speed = walkSpeed;
        agent.stoppingDistance = stopDistance;
        maxHealth = health;
    }


    void Update()
    {
        //由血量判斷階段
        if (health / maxHealth >= 0.6f) phase = 0;
        else if (health / maxHealth < 0.6f && health / maxHealth >= 0.3f) phase = 1;
        else phase = 2;

        //尋找最接近的玩家
        float minDistance = 100f;
        foreach(GameObject i in GameManager.Instance.playerList)
        {
            if (Vector3.Distance(gameObject.transform.position, i.transform.position) < minDistance)
            {
                minDistance = Vector3.Distance(gameObject.transform.position, i.transform.position);
                targetPlayer = i;
            }
        }

        //目前玩家與修羅的距離
        if(targetPlayer != null) currentDistance = Vector3.Distance(gameObject.transform.position, targetPlayer.transform.position);

        switch (BossState)
        {
            case State.Sleep:
                //Do nothing until player into detect range
                if (currentDistance <= detectRadius) 
                    BossState = State.Idle;
                break;

            case State.Idle:
                agent.SetDestination(targetPlayer.transform.position);

                if (currentDistance <= stopDistance)
                {
                    IdleTime -= Time.deltaTime;
                    walkAround(currentDistance);
                }

                if (IdleTime <= 0)
                {
                    IdleTime = Random.Range(1f, 5f);
                    BossState = State.Attack;
                }
                break;

            case State.Attack:
                //Attack
                
                randAttackMode = Random.Range(0f, 10f);
                randAttackMode = (int)randAttackMode;
                randCoin = Random.Range(0f, 4f);
                if (phase == 0) { Debug.Log("Attack Mode: " + randAttackMode % 3); }
                else { Debug.Log("Attack Mode: " + randAttackMode % 4); }
                if (phase == 0)
                {
                    if (randAttackMode % 3 == 0)
                    {
                        aModePrepare();
                        BossState = State.Mode_A;
                    }
                    else if (randAttackMode % 3 == 1)
                    {
                        delayTime = bModeDelay;
                        BossState = State.Mode_B;
                    }
                    else
                    {
                        delayTime = cModeDelay;
                        BossState = State.Mode_C;
                    }
                }
                else if (phase == 1)
                {
                    if (randAttackMode % 3 == 0)
                    {
                        //A>B
                        BossState = State.Mode_A;
                    }
                    else if (randAttackMode % 3 == 1)
                    {
                        //B>A mode
                        delayTime = bModeDelay;
                        BossState = State.Mode_B;
                    }
                    else if (randAttackMode % 3 == 2)
                    {
                        //C>A mode
                        BossState = State.Mode_C;
                    }
                    else
                    {
                        //C>B mode
                        BossState = State.Mode_C;
                    }
                }

                else
                {
                    stage = 0;
                    if (randAttackMode % 4 == 0)
                    {
                        //A>A>A+ mode
                        BossState = State.Mode_A;
                    }
                    else if (randAttackMode % 4 == 1)
                    {
                        //B>B>B+ mode
                        BossState = State.Mode_B;
                    }
                    else if (randAttackMode % 4 == 2)
                    {
                        //C>A>B mode
                        BossState = State.Mode_C;
                    }
                    else
                    {
                        //C>B>A mode  
                        BossState = State.Mode_C;
                    }
                }
                break;

            case State.Mode_A:
                //攻擊前的延遲
                if (delayTime > 0) delayTime -= Time.deltaTime;

                //判斷攻擊是否完成 (快速上升擊中玩家)
                if (delayTime <= 0 && triggerPrefab[0].transform.position.y <= targetPlayer.transform.position.y)
                    triggerPrefab[0].transform.position += new Vector3(0f, aSpeed, 0f) * Time.deltaTime;


                //完成攻擊後的切換狀態準備
                else if (delayTime <= 0 && triggerPrefab[0].transform.position.y >= targetPlayer.transform.position.y)
                {
                    Debug.Log("Attack Done!");
                    //移除A物件 以及從List中除名
                    Destroy(triggerPrefab[0]);
                    triggerPrefab.RemoveAt(0);

                    //階段1 切回Idle
                    if (phase == 0) BossState = State.Idle;
                    //階段2
                    else if (phase == 1)
                    {
                        coinGenerator();
                        //模式1 切至B狀態
                        if (randAttackMode % 4 == 0) BossState = State.Mode_B;
                        //其餘切回Idle
                        else BossState = State.Idle;
                    }
                    else
                    {
                        //coin生成判斷
                        coinGenerator();
                        stage++;
                        //模式1 持續執行A模式
                        if (randAttackMode % 4 == 0) aModePrepare();
                        //模式3 切至B狀態
                        else if (randAttackMode % 4 == 2) BossState = State.Mode_B;
                        //其餘(只有模式4) 切回Idle
                        else BossState = State.Idle;
                    }
                }
                break;

            case State.Mode_B:
                //攻擊前的延遲
                if(delayTime >= 0) delayTime -= Time.deltaTime;

                //生成子彈
                else if(triggerPrefab.Count == 0) bModePrepare();

                //子彈飛行
                else if (timer > 0)
                {
                    timer -= Time.deltaTime;
                    foreach (GameObject i in triggerPrefab)
                    {
                        i.transform.position += i.transform.forward * bSpeed * Time.deltaTime;
                    }
                }

                //完成攻擊後的切換狀態準備
                else
                {
                    Debug.Log("Attack Done!");
                    
                    //移除B物件 以及從List中除名
                    for (int i = 0; i < triggerPrefab.Count; i++) Destroy(triggerPrefab[0]);
                    foreach (GameObject i in triggerPrefab) triggerPrefab.Remove(i);

                    //階段1 切回Idle
                    if (phase == 0) BossState = State.Idle;

                    //階段2
                    else if (phase == 1)
                    {
                        coinGenerator();
                        //模式2 切至A模式
                        if (randAttackMode % 4 == 1) BossState = State.Mode_A;
                        //其餘狀況 切回Idle
                        else BossState = State.Idle;
                    }
                    else
                    {
                        //coin生成判斷
                        coinGenerator();
                        stage++;

                        //模式4 切至A狀態
                        if (randAttackMode % 4 == 3)
                        {
                            aModePrepare();
                            BossState = State.Mode_A;
                        }
                        //其餘狀況 運行至第三步驟時 回到Idle
                        else if(stage == 3) BossState = State.Idle;
                    }
                }

                break;

            case State.Mode_C:
                if (delayTime >= 0) delayTime -= Time.deltaTime;
                else cModePrepare();

                if (timer >= 0) timer -= Time.deltaTime;
                else
                {
                    agent.speed = walkSpeed;
                    agent.stoppingDistance = stopDistance;
                }
                Debug.Log("Attack Done!");

                if (phase == 0) BossState = State.Idle;
                else if(phase == 1)
                {
                    coinGenerator();
                    if (randAttackMode % 4 == 2)
                    {
                        aModePrepare();
                        BossState = State.Mode_A;
                    }
                    else BossState = State.Mode_B;
                }
                else
                {
                    coinGenerator();
                    if (randAttackMode % 4 == 2)
                    {
                        aModePrepare();
                        BossState = State.Mode_A;
                    }
                    else BossState = State.Mode_B;
                }
                break;

            default:
                break;
        }

        void walkAround(float distance)
        {
            if (((int)distance / 2) % 2 == 0)
            {
                agent.Move(gameObject.transform.right * walkSpeed / 2 * Time.deltaTime);
                //Debug.Log("Slowly moving right");
            }
            else
            {
                agent.Move(gameObject.transform.right * -1 * walkSpeed / 2 * Time.deltaTime);
                //Debug.Log("Slowly moving left");
            }
        }

        void aModePrepare()
        {
            delayTime = aModeDelay;
            timer = 0.1f;
            if(stage == 3 && randAttackMode % 4 == 0) triggerPrefab.Add(Instantiate(appModeAttack, targetPlayer.transform.position - new Vector3(0f, 2f, 0f), targetPlayer.transform.rotation));
            else triggerPrefab.Add(Instantiate(aModeAttack, targetPlayer.transform.position - new Vector3(0f, 2f, 0f), targetPlayer.transform.rotation));
        }

        void bModePrepare()
        {
            timer = bTimer;

            if (stage == 3 && randAttackMode % 4 == 1)
            {
                //正前方
                triggerPrefab.Add(Instantiate(bppModeAttack, gameObject.transform.position + gameObject.transform.forward * 2f, gameObject.transform.rotation));
                //右前方
                triggerPrefab.Add(Instantiate(bppModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(22.5f) * 2f * -1 + gameObject.transform.right * Mathf.Sin(22.5f) * 2f * -1, gameObject.transform.rotation * Quaternion.Euler(0f, 22.5f, 0f)));
                //左方
                triggerPrefab.Add(Instantiate(bppModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(45) * 2f + gameObject.transform.right * Mathf.Sin(45) * 2f * -1, gameObject.transform.rotation * Quaternion.Euler(0, -45f, 0)));
                //左前方
                triggerPrefab.Add(Instantiate(bppModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(22.5f) * 2f * -1 + gameObject.transform.right * Mathf.Sin(22.5f) * 2f, gameObject.transform.rotation * Quaternion.Euler(0, -22.5f, 0)));
                //右方
                triggerPrefab.Add(Instantiate(bppModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(45) * 2f + gameObject.transform.right * Mathf.Sin(45) * 2f, gameObject.transform.rotation * Quaternion.Euler(0, 45f, 0)));
            }
            else
            {
                //正前方
                triggerPrefab.Add(Instantiate(bModeAttack, gameObject.transform.position + gameObject.transform.forward * 2f, gameObject.transform.rotation));
                //右前方
                triggerPrefab.Add(Instantiate(bModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(22.5f) * 2f * -1 + gameObject.transform.right * Mathf.Sin(22.5f) * 2f * -1, gameObject.transform.rotation * Quaternion.Euler(0f, 22.5f, 0f)));
                //左方
                triggerPrefab.Add(Instantiate(bModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(45) * 2f + gameObject.transform.right * Mathf.Sin(45) * 2f * -1, gameObject.transform.rotation * Quaternion.Euler(0, -45f, 0)));
                //左前方
                triggerPrefab.Add(Instantiate(bModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(22.5f) * 2f * -1 + gameObject.transform.right * Mathf.Sin(22.5f) * 2f, gameObject.transform.rotation * Quaternion.Euler(0, -22.5f, 0)));
                //右方
                triggerPrefab.Add(Instantiate(bModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(45) * 2f + gameObject.transform.right * Mathf.Sin(45) * 2f, gameObject.transform.rotation * Quaternion.Euler(0, 45f, 0)));
            }
        }

        void cModePrepare()
        {
            timer = cTimer;
            agent.speed = rushSpeed;
            agent.stoppingDistance = rushRadius;
        }

        void coinGenerator()
        {
            //用randCoin做生成判斷
        }
    }
}


enum State
{
    Sleep, Idle, Attack, Mode_A, Mode_B, Mode_C
}
