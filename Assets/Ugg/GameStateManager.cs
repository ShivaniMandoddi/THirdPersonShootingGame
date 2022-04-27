using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class GameStateManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameStateManager instance;

    NavMeshAgent agent;
    Animator animator;
    public Transform player;
    public State currentState;
    //public GameObject bloodEffect;
    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
          
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        agent.enabled = true;
        
        currentState = new Idle(this.gameObject, agent, animator, player);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        currentState = currentState.Process();
        
    }
   
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            //bloodEffect.transform.position = other.gameObject.transform.position;
            //Instantiate(bloodEffect, other.gameObject.transform.position, Quaternion.identity);
            other.gameObject.SetActive(false);
            //this.gameObject.SetActive(false);
            currentState = new Death(this.gameObject, agent, animator, player);

        }
    }
}
public class State
{
    public enum STATE
    {
        ATTACK, CHASE, IDLE, DEATH, WONDER
    }
    public enum EVENTS
    {
        ENTER, UPDATE, EXIT
    }
    public STATE stateName;
    public EVENTS eventStage;

    public GameObject nPC;
    public NavMeshAgent agent;
    public Animator animator;
    public Transform playerPosition;
    public State nextState;
  
    public State(GameObject _npc, NavMeshAgent _agent, Animator _animator, Transform _playerPosition)
    {
        this.nPC = _npc;
        this.playerPosition = _playerPosition;
        this.agent = _agent;
        this.animator = _animator;
        eventStage = EVENTS.ENTER;
    }
    public virtual void Enter()
    {
        eventStage = EVENTS.UPDATE;
    }
    public virtual void Update()
    {
        eventStage = EVENTS.UPDATE;
    }
    public virtual void Exit()
    {
        eventStage = EVENTS.EXIT;
    }
    public State Process()
    {
        if (eventStage == EVENTS.ENTER)
        {
            Enter();
        }
        if (eventStage == EVENTS.UPDATE)
        {
            Update();
        }
        if (eventStage == EVENTS.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }
    public bool CanSeePlayer()
    {
        if (Vector3.Distance(nPC.transform.position, playerPosition.position) < 15f)
            return true;
        return false;
    }
    
    
}
public class Idle: State
{
    public Idle(GameObject _npc, NavMeshAgent _agent, Animator _animator, Transform _playerPosition) : base(_npc, _agent, _animator, _playerPosition)
    {
        stateName = STATE.IDLE;
        agent.enabled = true;
    }
    public override void Enter()
    {
        animator.SetTrigger("isIdle");
        base.Enter();

    }
    public override void Update()
    {
        if (CanSeePlayer())
        {
            nextState = new Chase(nPC, agent, animator, playerPosition);
            eventStage = EVENTS.EXIT;
        }
        else
        {
            nextState = new Wonder(nPC, agent, animator, playerPosition);
            eventStage = EVENTS.EXIT;
        }
        // base.Update();
    }
    public override void Exit()
    {
        animator.ResetTrigger("isIdle");
        base.Exit();
    }


}
public class Wonder: State
{
    public Wonder(GameObject _npc, NavMeshAgent _agent, Animator _animator, Transform _playerPosition) : base(_npc, _agent, _animator, _playerPosition)
    {
        stateName = STATE.WONDER;
        
    }
    public override void Enter()
    {
        animator.SetTrigger("isWalking");
        base.Enter();

    }
    public override void Update()
    {
        float randValueX = nPC.transform.position.x + Random.Range(-5f, 5f);
        float randValueZ = nPC.transform.position.z + Random.Range(-5f, 5f);
        float ValueY = Terrain.activeTerrain.SampleHeight(new Vector3(randValueX, 0f, randValueZ));
        Vector3 destination = new Vector3(randValueX, ValueY, randValueZ);
        if (CanSeePlayer())
        {
            nextState = new Chase(nPC, agent, animator, playerPosition);
            eventStage = EVENTS.EXIT;
        }
        // base.Update();
    }
    public override void Exit()
    {
        animator.ResetTrigger("isWalking");
        base.Exit();
    }
}
public class Chase : State
{
    public Chase(GameObject _npc, NavMeshAgent _agent, Animator _animator, Transform _playerPosition) : base(_npc, _agent, _animator, _playerPosition)
    {
        stateName = STATE.CHASE;
        agent.stoppingDistance = 5f;

    }
    public override void Enter()
    {
        animator.SetTrigger("isRunning");
        base.Enter();

    }
    public override void Update()
    {
        agent.SetDestination(playerPosition.position);
        nPC.transform.LookAt(playerPosition.position);
      
        if (!CanSeePlayer())
        {
            nextState = new Wonder(nPC, agent, animator, playerPosition);
            eventStage = EVENTS.EXIT;
        }
        if (Vector3.Distance(nPC.transform.position,playerPosition.position)<=agent.stoppingDistance)
        {
            nextState = new Attack(nPC, agent, animator, playerPosition);
            eventStage = EVENTS.EXIT;
        }

        // base.Update();
    }
    public override void Exit()
    {
        animator.ResetTrigger("isRunning");
        base.Exit();
    }


}
public class Attack : State
{
    public Attack(GameObject _npc, NavMeshAgent _agent, Animator _animator, Transform _playerPosition) : base(_npc, _agent, _animator, _playerPosition)
    {
        stateName = STATE.ATTACK;
        nPC.transform.LookAt(playerPosition.position);
    }
    public override void Enter()
    {
        animator.SetTrigger("isShooting");
        base.Enter();

    }
    public override void Update()
    {
       if(Vector3.Distance(nPC.transform.position,playerPosition.position)>agent.stoppingDistance+1f)
        {
            nextState = new Idle(nPC, agent, animator, playerPosition);
            eventStage = EVENTS.EXIT;
        }
        // base.Update();
    }
    public override void Exit()
    {
        animator.ResetTrigger("isShooting");
        base.Exit();
    }


}
public class Death : State
{
    float time;
    public Death(GameObject _npc, NavMeshAgent _agent, Animator _animator, Transform _playerPosition) : base(_npc, _agent, _animator, _playerPosition)
    {
        stateName = STATE.DEATH;
       agent.enabled = false;
        
    }
    public override void Enter()
    {
        animator.SetTrigger("isSleeping");
        base.Enter();

    }
    public override void Update()
    {
         time = time + Time.deltaTime;
         if(time>5f)
         {
             agent.enabled = true;
            nextState = new Idle(nPC, agent, animator, playerPosition);
            nPC.SetActive(false);
            eventStage = EVENTS.EXIT;
        }
        

    }
    public override void Exit()
    {
        animator.ResetTrigger("isSleeping");
        
         base.Exit();
    }


}



