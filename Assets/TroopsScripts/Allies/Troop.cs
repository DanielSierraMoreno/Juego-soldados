using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Troop : MonoBehaviour
{
    public enum Type { PAWN, ARCHER, LANCER, SOLDIER};

    public float moveSpeed = 5;

    public Animator anim;

    public Type troopType;

    public bool selected = false;

	public NavMeshAgent agent;

	public enum StateArmy { MOVING, IDLE, ATTACKING, DEFENSE };
	public StateArmy stateArmy;

	public bool targetAttack = false;

	public AttackPoints currentTarget;
	public bool isAttacking = false;
	public bool isDoingAttack = false;

	protected float lastAttackTime = 0;

	public float delayBetweenAttacks = 0.5f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	protected virtual void Start()
	{

    }

	// Update is called once per frame
	protected virtual void Update()
    {
        


    }

	public void SetTarget(AttackPoints target)
	{
		currentTarget = target;
		isAttacking = true;
		target.assignedTroops.Add(this);
		// Aquí podrías mover el NavMeshAgent hacia target
	}

	public void ClearTarget()
	{
		stateArmy = StateArmy.IDLE;
		if (currentTarget != null)
		{
			currentTarget.assignedTroops.Remove(this);
		}
		currentTarget = null;
		isAttacking = false;
	}

	public virtual void Move(Vector2 dir)
	{
        if(dir.magnitude > 0)
        {
            anim.SetBool("Move", true);
        }
        else
        {
			anim.SetBool("Move", false);

		}

        if(dir.x > 0)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }
        else if(dir.x < 0)
        {
			this.transform.localScale = new Vector3(-1, 1, 1);
		}

		this.GetComponent<Rigidbody2D>().MovePosition(this.GetComponent<Rigidbody2D>().position + new Vector2(dir.x * moveSpeed * Time.deltaTime, dir.y * moveSpeed * Time.deltaTime));
	}

    public virtual void DeSelect()
    {

    }
	public virtual void Select()
	{
		ClearTarget();
	}


	public virtual void SetStopDefense()
	{

	}
	public virtual void SetDefense()
	{

	}

	public virtual void Attack(Vector3 dir)
	{

	}
}
