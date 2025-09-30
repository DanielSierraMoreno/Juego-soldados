using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class Warrior : Troop
{

	public float attackRange = 1.5f;



	// Start is called once before the first execution of Update after the MonoBehaviour is created
	protected override void Start()
	{
		base.Start();
		stateArmy = StateArmy.IDLE;
		troopType = Type.SOLDIER;
		agent.speed = moveSpeed;
		agent.updateRotation = false;
		agent.updateUpAxis = false;

	}

	// Update is called once per frame
	protected override void Update()
	{
		if (!selected)
		{
			if (isDoingAttack) { return; }




			if (stateArmy != StateArmy.DEFENSE)
			{
				if (agent.destination.x - transform.position.x > 0)
				{
					this.transform.GetChild(0).localScale = new Vector3(1, 1, 1);
				}
				else if (agent.destination.x - transform.position.x < 0)
				{
					this.transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
				}
			}


			switch(ArmyManager.Instance.globalOrder)
			{
				case ArmyManager.GlobalOrders.RETREAT:

					break;
				case ArmyManager.GlobalOrders.DEFENSE:

					agent.radius = 0.1f;
					if (stateArmy != StateArmy.DEFENSE)
					{
						agent.isStopped = false;

						agent.SetDestination(ArmyManager.Instance.DefensePointUnoccuped(this.transform.position));
						anim.CrossFade("Run", 0);
					}

					break;
				case ArmyManager.GlobalOrders.ATTACK:


					agent.SetDestination(currentTarget.transform.position);

					if (stateArmy != StateArmy.ATTACKING && currentTarget != null)
					{
						agent.radius = 0.25f;

						agent.isStopped = false;
					}

					if (Vector3.Distance(this.transform.position, agent.destination) < attackRange)
					{
						agent.isStopped = true;
						anim.CrossFade("Idle", 0);

						stateArmy = StateArmy.ATTACKING;
					}
					else
					{
						stateArmy = StateArmy.IDLE;

						anim.CrossFade("Run", 0);

					}

					if (stateArmy == StateArmy.ATTACKING)
					{

					}


					break;
			}


		}
	}

	public override void Move(Vector2 dir)
	{
		if (isDoingAttack) { return; }


		if (dir.magnitude > 0.2f)
		{
			stateArmy = StateArmy.MOVING;

			anim.CrossFade("Run", 0);
		}
		else if (stateArmy == StateArmy.MOVING)
		{
			stateArmy = StateArmy.IDLE;

			anim.CrossFade("Idle", 0);
			return;

		}

		if (dir.x > 0)
		{
			this.transform.GetChild(0).localScale = new Vector3(1, 1, 1);
		}
		else if (dir.x < 0)
		{
			this.transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
		}
		Vector3 targetPosition = transform.position + new Vector3(dir.x, dir.y, 0) * 15 * Time.deltaTime;
		agent.isStopped = false;
		agent.SetDestination(targetPosition);

	}
	public override void DeSelect()
	{
		selected = false;

		agent.isStopped = false;
	}
	public override void Select()
	{
		base.Select();
		selected = true;

		agent.isStopped = true;
		anim.CrossFade("Idle", 0);

	}
	public override void SetDefense()
	{

		stateArmy = StateArmy.DEFENSE;
	}
	public override void SetStopDefense()
	{

		stateArmy = StateArmy.MOVING;
	}
	public void Reset()
	{
		stateArmy = StateArmy.IDLE;
		selected = false;
		agent.isStopped = false;
		anim.CrossFade("Idle", 0);
	}

	public override void Attack(Vector3 dir)
	{
		if (isDoingAttack) { return; }

		if (dir.x > 0)
		{
			this.transform.GetChild(0).localScale = new Vector3(1, 1, 1);
		}
		else if (dir.x < 0)
		{
			this.transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
		}


		isDoingAttack = true;
		anim.CrossFade("Attack", 0);

		if (!selected)
			StartCoroutine(DoAttack(currentTarget));
		else
			StartCoroutine(DoAttack());


	}

	private IEnumerator DoAttack(AttackPoints target = null)
	{
		yield return new WaitForSeconds(0.2f);

		if (target != null) 
		{ 		
			target.Destroy();
			target.gameObject.SetActive(false);	
		}



		yield return new WaitForSeconds(0.25f);
		anim.CrossFade("Idle", 0);

		yield return new WaitForSeconds(0.2f);
		isDoingAttack = false;

	}
}
