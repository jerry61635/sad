using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoulMovement : MonoBehaviour
{
    //���A�ŧi
    enum State
    {
        /* ���B���U���A�Ը�
         * Sleep:   �ίv     ���a�i�J�ĤH�����d�򤺤��e AI���|�@�X����ʧ@
         * Idle:    �ݾ�     �b�d�򤺦����a�� �i�J�����e���������j����
         * Requim:  �l�   �ϥΫ�|�N�ؼЪ��a���� "unknown.cs" �}��
         * Poison1: �m�R�r�� �I��e���ݨ�� ���ۨ�30��줺�����a�N�C�����20�ˮ`
         * Poison2: �G���G�� 
         * Run:     �}�s���� �[�t�ðl�v�ؼЪ��a �ù�ؼШϥ�Poison2 Melee ���򤭬�
         * Melee:   �ѭ��x�� ���{���q���� ���ݨ��
         * 
         * ��AI����O�Ҧ����a�����a�S��¾�~ 
         */
        Sleep, Idle, Requiem, Poison1, Poison2, Run, Melee
    }

    //Global Reference
    NavMeshAgent agent;
    [SerializeField]
    State BossState;

    //AI�ƭȳ]�w
    public float stopDistance;  //�P���a�O�����Z��
    public float runSpeed;      //Run��Soul�����ʳt��
    public float detectRadius;
    public int targetPlayerIndex;
    public float idleDelay;
    public float requiemDelay;
    public float poison1Delay;
    public float poison2Delay;
    public float runTime;

    //Constant variable
    float randAttackMode; //
    float timer;                 //�Ω�p�ɬ������ܼ�
    float currentDistance;
    float[] playerDistance = new float[4];

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        BossState = State.Sleep;
        agent.speed = 0f;
        agent.stoppingDistance = stopDistance;
        timer = idleDelay;                              //�_�lIdle�ɶ�
    }

    // Update is called once per frame
    void Update()
    {        //Debug.Log(timer);
        //�M��̱��񪺪��a
        float minDistance = 100f;
        int j = 0;
        if (GameManager.Instance.playerList.Count != 0) {
            foreach (GameObject i in GameManager.Instance.playerList)
            {
                playerDistance[j] = Vector3.Distance(gameObject.transform.position, i.transform.position); //��s�C�Ӫ��a�PSoul���Z��
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
                List<PlayerInSoul> targetList = new List<PlayerInSoul>(); //�Ω��^���|��������a
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
                //�r��
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
                //�G��
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
                //�ߤ@�첾
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
                //���q����
                Debug.Log("Attack Done! Going back Idle.");
                timer = idleDelay;
                BossState = State.Idle;
                break;
        }
    }
    
}

