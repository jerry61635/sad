using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShuraMovement : MonoBehaviour
{
    public GameObject aModeAttack;
    public GameObject bModeAttack;

    public List<GameObject> triggerPrefab = new List<GameObject>();

    public float phase = 0;  //血量60%以上第一階段 30%以上第二階段 30%以下最終階段
    public float health = 100f;
    float maxHealth;
    [SerializeField]
    State BossState = State.Idle;

    public float speed;
    public float Idle_time;
    public NavMeshAgent agent;
    public float detectRadius;
    public float stopDistance;
    public GameObject targetPlayer;
    float currentDistance = 100f;
    float timer;
    float delayTime;

    float randAttackMode;
    float randCoin;

    // Start is called before the first frame update
    void Start()
    {
        agent.speed = speed;
        agent.stoppingDistance = stopDistance;
        maxHealth = health;
    }

    // Update is called once per frame
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
                    Idle_time -= Time.deltaTime;
                    walkAround(currentDistance);
                }

                if (Idle_time <= 0)
                {
                    Idle_time = Random.Range(1f, 5f);
                    BossState = State.Attack;
                }
                break;

            case State.Attack:
                //Attack
                
                randAttackMode = Random.Range(0f, 10f);
                randAttackMode = (int)randAttackMode;
                randCoin = Random.Range(0f, 4f);
                timer -= Time.deltaTime;
                if (phase == 0) { Debug.Log("Attack Mode: " + randAttackMode % 3); }
                else { Debug.Log("Attack Mode: " + randAttackMode % 4); }
                if (phase == 0)
                {
                    if (randAttackMode % 3 == 0)
                    {
                        delayTime = 1f;
                        timer = 0.1f;
                        triggerPrefab.Add(Instantiate(aModeAttack, targetPlayer.transform.position - new Vector3(0f, 2f, 0f), targetPlayer.transform.rotation));
                        BossState = State.Mode_A;
                    }
                    else if (randAttackMode % 3 == 1)
                    {
                        timer = 1f;
                        //正前方
                        triggerPrefab.Add(Instantiate(bModeAttack, gameObject.transform.position + gameObject.transform.forward * 2f, gameObject.transform.rotation));
                        //右前方
                        triggerPrefab.Add(Instantiate(bModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(22.5f) * 2f * -1 + gameObject.transform.right * Mathf.Sin(22.5f) * 2f * -1, gameObject.transform.rotation * Quaternion.Euler(0f,22.5f,0f)));
                        //左方
                        triggerPrefab.Add(Instantiate(bModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(45) * 2f + gameObject.transform.right * Mathf.Sin(45) * 2f * -1, gameObject.transform.rotation * Quaternion.Euler(0, -45f, 0)));
                        //左前方
                        triggerPrefab.Add(Instantiate(bModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(22.5f) * 2f * -1 + gameObject.transform.right * Mathf.Sin(22.5f) * 2f, gameObject.transform.rotation * Quaternion.Euler(0, -22.5f, 0)));
                        //右方
                        triggerPrefab.Add(Instantiate(bModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(45) * 2f + gameObject.transform.right * Mathf.Sin(45) * 2f, gameObject.transform.rotation * Quaternion.Euler(0, 45f, 0)));
                        BossState = State.Mode_B;
                    }
                    else
                    {
                        BossState = State.Mode_C;
                    }
                }
                else if (phase == 1)
                {
                    if (randAttackMode % 3 == 0)
                    {
                        //A>B
                        BossState = State.Mode_A;
                        BossState = State.Mode_B;
                    }
                    else if (randAttackMode % 3 == 1)
                    {
                        //B>A mode
                        BossState = State.Mode_B;
                        BossState = State.Mode_A;
                    }
                    else if (randAttackMode % 3 == 2)
                    {
                        //C>A mode
                        BossState = State.Mode_C;
                        BossState = State.Mode_A;
                    }
                    else
                    {
                        //C>B mode
                        BossState = State.Mode_C;
                        BossState = State.Mode_B;
                    }

                    if(randCoin % 2 == 0)
                    {
                        //Instaniate coin
                    }
                }

                else
                {
                    if (randAttackMode % 3 == 0)
                    {
                        //A>A>A+ mode
                        BossState = State.Mode_A;
                        randCoin = Random.Range(0f, 4f);
                        BossState = State.Mode_A;
                        randCoin = Random.Range(0f, 4f);
                        BossState = State.Mode_App;
                        randCoin = Random.Range(0f, 4f);
                    }
                    else if (randAttackMode % 3 == 1)
                    {
                        //B>B>B+ mode
                        BossState = State.Mode_B;
                        randCoin = Random.Range(0f, 4f);
                        BossState = State.Mode_B;
                        randCoin = Random.Range(0f, 4f);
                        BossState = State.Mode_Bpp;
                        randCoin = Random.Range(0f, 4f);
                    }
                    else if (randAttackMode % 3 == 2)
                    {
                        //C>A>B mode
                        BossState = State.Mode_C;
                        randCoin = Random.Range(0f, 4f);
                        BossState = State.Mode_A;
                        randCoin = Random.Range(0f, 4f);
                        BossState = State.Mode_B;
                        randCoin = Random.Range(0f, 4f);
                    }
                    else
                    {
                        //C>B>A mode  
                    }
                }
                break;

            case State.Mode_A:

                if (delayTime > 0)
                {
                    delayTime -= Time.deltaTime;
                    //Debug.Log("Time left: " + delayTime);
                }

                if (delayTime <= 0 && triggerPrefab[0].transform.position.y <= targetPlayer.transform.position.y)
                {
                    triggerPrefab[0].transform.position += new Vector3(0f, 20f, 0f) * Time.deltaTime;
                }
                else if (delayTime <= 0 && triggerPrefab[0].transform.position.y >= targetPlayer.transform.position.y)
                {
                    Debug.Log("Attack Done!");
                    Destroy(triggerPrefab[0]);
                    triggerPrefab.RemoveAt(0);
                    if (phase == 0) BossState = State.Idle;
                    else if (phase == 1)
                    {
                        if (randAttackMode % 4 == 0) BossState = State.Mode_B;
                        else BossState = State.Idle;
                    }
                    else
                    {
                        if (randAttackMode % 4 == 0)
                        {
                            delayTime = 1f;
                            timer = 0.1f;
                            triggerPrefab.Add(Instantiate(aModeAttack, targetPlayer.transform.position - new Vector3(0f, 2f, 0f), targetPlayer.transform.rotation));
                        }
                        else if (randAttackMode % 4 == 2) BossState = State.Mode_B;
                        else BossState = State.Idle;
                    }
                }
                break;

            case State.Mode_B:
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                    foreach (GameObject i in triggerPrefab)
                    {
                        i.transform.position += i.transform.forward * 5f * Time.deltaTime;
                    }
                }

                else
                {
                    Debug.Log("Attack Done!");
                    
                    for (int i = 0; i < triggerPrefab.Count; i++) Destroy(triggerPrefab[0]);
                    foreach (GameObject i in triggerPrefab) triggerPrefab.Remove(i);
                    
                    if (phase == 0) BossState = State.Idle;
                    else if(phase == 1)
                    {
                        if (randAttackMode % 2 == 1) BossState = State.Mode_A;
                        else BossState = State.Idle;
                    }
                }

                break;

            case State.Mode_C:
                Debug.Log("Attack Done!");
                timer = 2;
                BossState = State.Idle;

                break;

            default:
                break;
        }

        void walkAround(float distance)
        {
            if (((int)distance / 2) % 2 == 0)
            {
                agent.Move(gameObject.transform.right * speed / 2 * Time.deltaTime);
                //Debug.Log("Slowly moving right");
            }
            else
            {
                agent.Move(gameObject.transform.right * -1 * speed / 2 * Time.deltaTime);
                //Debug.Log("Slowly moving left");
            }
        }
    }
}


enum State
{
    Sleep, Idle, Attack, Mode_A, Mode_B, Mode_C, Mode_App, Mode_Bpp
}
