using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShuraMovement : MonoBehaviour
{
    public float health = 100f;
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

    // Start is called before the first frame update
    void Start()
    {
        agent.speed = speed;
        agent.stoppingDistance = stopDistance;
    }

    // Update is called once per frame
    void Update()
    {
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

        //current distance between player & Shura
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
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    Debug.Log("Attack Done!");
                    timer = 2f;
                    BossState = State.Idle;
                }
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
    Sleep,Idle, Attack
}
