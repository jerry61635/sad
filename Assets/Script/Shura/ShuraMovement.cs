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

    //�`��
    float timer;
    float delayTime;
    float IdleTime;
    float currentDistance;
    float randAttackMode;
    float randCoin;
    public GameObject targetPlayer;

    public List<GameObject> triggerPrefab = new List<GameObject>();

    public float phase = 0;  //��q60%�H�W�Ĥ@���q 30%�H�W�ĤG���q 30%�H�U�̲׶��q
    public float health = 100f;
    float maxHealth;
    [SerializeField]
    State BossState = State.Sleep;

    public float aModeDelay;    //A���A�������e����
    public float aSpeed;        //A�������W�ɳt��
    public float bModeDelay;    //B���A�������e����
    public float bSpeed;        //B�������l�u����t��
    public float bTimer;        //B�������l�u����ɶ�
    public float cModeDelay;
    public float cTimer;
    public float rushSpeed;     //C���A���Ĩ�t��
    public float rushRadius;
    public float stage;         //�ΥH���Ѷ��q�T���B�涥�q

    //Agent reference
    public NavMeshAgent agent;
    public float detectRadius;  //��ù�d���������Z��
    public float walkSpeed;     //��ù���樫�t��
    public float stopDistance;  //��ù�P���a�����w�]�Z��

    void Start()
    {
        IdleTime = 2f;
        agent.speed = walkSpeed;
        agent.stoppingDistance = stopDistance;
        maxHealth = health;
    }


    void Update()
    {
        //�Ѧ�q�P�_���q
        if (health / maxHealth >= 0.6f) phase = 0;
        else if (health / maxHealth < 0.6f && health / maxHealth >= 0.3f) phase = 1;
        else phase = 2;

        //�M��̱��񪺪��a
        float minDistance = 100f;
        foreach(GameObject i in GameManager.Instance.playerList)
        {
            if (Vector3.Distance(gameObject.transform.position, i.transform.position) < minDistance)
            {
                minDistance = Vector3.Distance(gameObject.transform.position, i.transform.position);
                targetPlayer = i;
            }
        }

        //�ثe���a�P��ù���Z��
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
                //�����e������
                if (delayTime > 0) delayTime -= Time.deltaTime;

                //�P�_�����O�_���� (�ֳt�W���������a)
                if (delayTime <= 0 && triggerPrefab[0].transform.position.y <= targetPlayer.transform.position.y)
                    triggerPrefab[0].transform.position += new Vector3(0f, aSpeed, 0f) * Time.deltaTime;


                //���������᪺�������A�ǳ�
                else if (delayTime <= 0 && triggerPrefab[0].transform.position.y >= targetPlayer.transform.position.y)
                {
                    Debug.Log("Attack Done!");
                    //����A���� �H�αqList�����W
                    Destroy(triggerPrefab[0]);
                    triggerPrefab.RemoveAt(0);

                    //���q1 ���^Idle
                    if (phase == 0) BossState = State.Idle;
                    //���q2
                    else if (phase == 1)
                    {
                        coinGenerator();
                        //�Ҧ�1 ����B���A
                        if (randAttackMode % 4 == 0) BossState = State.Mode_B;
                        //��l���^Idle
                        else BossState = State.Idle;
                    }
                    else
                    {
                        //coin�ͦ��P�_
                        coinGenerator();
                        stage++;
                        //�Ҧ�1 �������A�Ҧ�
                        if (randAttackMode % 4 == 0) aModePrepare();
                        //�Ҧ�3 ����B���A
                        else if (randAttackMode % 4 == 2) BossState = State.Mode_B;
                        //��l(�u���Ҧ�4) ���^Idle
                        else BossState = State.Idle;
                    }
                }
                break;

            case State.Mode_B:
                //�����e������
                if(delayTime >= 0) delayTime -= Time.deltaTime;

                //�ͦ��l�u
                else if(triggerPrefab.Count == 0) bModePrepare();

                //�l�u����
                else if (timer > 0)
                {
                    timer -= Time.deltaTime;
                    foreach (GameObject i in triggerPrefab)
                    {
                        i.transform.position += i.transform.forward * bSpeed * Time.deltaTime;
                    }
                }

                //���������᪺�������A�ǳ�
                else
                {
                    Debug.Log("Attack Done!");
                    
                    //����B���� �H�αqList�����W
                    for (int i = 0; i < triggerPrefab.Count; i++) Destroy(triggerPrefab[0]);
                    foreach (GameObject i in triggerPrefab) triggerPrefab.Remove(i);

                    //���q1 ���^Idle
                    if (phase == 0) BossState = State.Idle;

                    //���q2
                    else if (phase == 1)
                    {
                        coinGenerator();
                        //�Ҧ�2 ����A�Ҧ�
                        if (randAttackMode % 4 == 1) BossState = State.Mode_A;
                        //��l���p ���^Idle
                        else BossState = State.Idle;
                    }
                    else
                    {
                        //coin�ͦ��P�_
                        coinGenerator();
                        stage++;

                        //�Ҧ�4 ����A���A
                        if (randAttackMode % 4 == 3)
                        {
                            aModePrepare();
                            BossState = State.Mode_A;
                        }
                        //��l���p �B��ܲĤT�B�J�� �^��Idle
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
                //���e��
                triggerPrefab.Add(Instantiate(bppModeAttack, gameObject.transform.position + gameObject.transform.forward * 2f, gameObject.transform.rotation));
                //�k�e��
                triggerPrefab.Add(Instantiate(bppModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(22.5f) * 2f * -1 + gameObject.transform.right * Mathf.Sin(22.5f) * 2f * -1, gameObject.transform.rotation * Quaternion.Euler(0f, 22.5f, 0f)));
                //����
                triggerPrefab.Add(Instantiate(bppModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(45) * 2f + gameObject.transform.right * Mathf.Sin(45) * 2f * -1, gameObject.transform.rotation * Quaternion.Euler(0, -45f, 0)));
                //���e��
                triggerPrefab.Add(Instantiate(bppModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(22.5f) * 2f * -1 + gameObject.transform.right * Mathf.Sin(22.5f) * 2f, gameObject.transform.rotation * Quaternion.Euler(0, -22.5f, 0)));
                //�k��
                triggerPrefab.Add(Instantiate(bppModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(45) * 2f + gameObject.transform.right * Mathf.Sin(45) * 2f, gameObject.transform.rotation * Quaternion.Euler(0, 45f, 0)));
            }
            else
            {
                //���e��
                triggerPrefab.Add(Instantiate(bModeAttack, gameObject.transform.position + gameObject.transform.forward * 2f, gameObject.transform.rotation));
                //�k�e��
                triggerPrefab.Add(Instantiate(bModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(22.5f) * 2f * -1 + gameObject.transform.right * Mathf.Sin(22.5f) * 2f * -1, gameObject.transform.rotation * Quaternion.Euler(0f, 22.5f, 0f)));
                //����
                triggerPrefab.Add(Instantiate(bModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(45) * 2f + gameObject.transform.right * Mathf.Sin(45) * 2f * -1, gameObject.transform.rotation * Quaternion.Euler(0, -45f, 0)));
                //���e��
                triggerPrefab.Add(Instantiate(bModeAttack, gameObject.transform.position + gameObject.transform.forward * Mathf.Cos(22.5f) * 2f * -1 + gameObject.transform.right * Mathf.Sin(22.5f) * 2f, gameObject.transform.rotation * Quaternion.Euler(0, -22.5f, 0)));
                //�k��
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
            //��randCoin���ͦ��P�_
        }
    }
}


enum State
{
    Sleep, Idle, Attack, Mode_A, Mode_B, Mode_C
}
