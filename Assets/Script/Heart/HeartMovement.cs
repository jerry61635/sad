using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HeartMovement : MonoBehaviour
{
    enum State
    {
        /*  Heart �欰�Ҧ�
         *  
         *  ��ŵ��ŧ�G�e�n���Ẵ�ʰ��͡A�Ҧ��b�����a�H�Ĥ謰���ߡA�V���y���@���D�����ء����t�׫��򢴬�A���ޯ঳������N�o�ɶ��C
         *  
         *  �L��d�G��e�����N�o��Ŧ�����a�A�ߧY�}�T���a����A�ޯ�C������Ĳ�o�@���C
         *  
         *  �V�ήL��G�H��e�Ҧ����a�U�۩Ҧb�a����ߡA�󢴤��ض�νd�򤺼��إV�ήL��A�@���Y���a�����}���d��A���a���ʳt�״�w�����H
         *  �Ҩ��ˮ`���ɢ����H�A�åB�C����r�߶ˮ`�����I�ͩR���򢴬�C���ޯ঳������N�o�ɶ��C
         *  
         *  ������x�]���q�����^�G�W�O����ƨg�{�Φa���H���ͪi�ʡA�H�ĤH����ߡA�V�e�袸���ר��d�򤺧���
         *  ���ܢ������ءC���ޯ঳������N�o�ɶ��C
         *  
         *  ����󰳡G�e�n���Ẵ�ʰ��͡A�Ҧ��b�����a�H�Ĥ謰���ߡA�V���y���@�����ء����t�׫��򢲬�
         *  �ýļ��V���ۨ��̪񤧪��a�y���������ˮ`�C���ޯ঳������N�o�ɶ��C
         */
        Sleep, Idle, Pull, Stun, Parasite, Melee, Rush
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

    [Header("Idle Settings")]
    public float idleDelay;

    [Header("��ŵ��ŧ")]
    public float pullingForce;
    public float pullingTime;

    [Header("�L��d")]
    public float stunTime;

    [Header("�V�ήL��")]
    public float parasiteDelay;
    public float parasiteDistance;
    public float parasiteTime;

    [Header("������x")]
    public float meleeDelay;

    [Header("�����")]
    public float rushDelay;
    public float rushPullingTime;
    public float rushPullingForce;
    public float rushSpeed;
    public float rushDistance;

    //Constant variable
    float randAttackMode;        //�M�w����
    float timer;                 //�Ω�p�ɬ������ܼ�
    int stage;                   //�Ω�Y�Ǫ��A���h���ϥέp�ɾ��ɨϥ�
    float[] playerDistance = new float[4]; //�x�s�C�Ӫ��a�PHeart���Z��

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
                float minDistance = 1000f;      //�Ω��x�s�̵u�Z�����ܼ�
                //�M��̱��񪺪��a �åB�x�s���a�PHeart���Z��
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
                //Idle���A���d���ɶ�
                if (timer > 0) timer -= Time.deltaTime;
                //�����Ҧ����
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
                //�ޯ����ɶ�
                if (timer > 0)
                {
                    foreach (Player i in GameManager.Instance.playerData)
                    {
                        i.controller.Move((gameObject.transform.position - i.transform.position).normalized * Time.deltaTime * pullingForce);
                    }
                    timer -= Time.deltaTime;
                }
                //��^Idle
                else
                {
                    timer = idleDelay;
                    BossState = State.Idle;
                }
                break;
            case State.Stun:
                //�w�t���a
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
                //��^Idle
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
                //Stage 0:�e�n�ǳ�(Movement Delay)
                //Stage 1:�ޯ����ɶ�
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                    if (stage == 1)
                    {
                        foreach (Player i in GameManager.Instance.playerData) //���o���a��T(Transform, Controller)
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
                    //�e�nŪ����
                    if (stage == 0) { timer = rushPullingTime; }
                    //�ޯ�I�񧹦�
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
