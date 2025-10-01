using System.Collections;
using UnityEngine;

public class TorchEnemy : EnemyTroops
{

	public float attackRange = 1.5f;



	// Start is called once before the first execution of Update after the MonoBehaviour is created
	protected override void Start()
	{
		base.Start();
		stateArmy = StateArmy.IDLE;
		troopType = Type.TORCH;
		agent.speed = moveSpeed;
		agent.updateRotation = false;
		agent.updateUpAxis = false;

	}

	// Update is called once per frame
	protected override void Update()
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


			switch (EnemyArmyManager.Instance.globalOrder)
			{
				case EnemyArmyManager.GlobalOrders.RETREAT:

					break;
				case EnemyArmyManager.GlobalOrders.DEFENSE:

					agent.radius = 0.1f;
					if (stateArmy != StateArmy.DEFENSE)
					{
						agent.isStopped = false;

						agent.SetDestination(EnemyArmyManager.Instance.DefensePointUnoccuped(this.transform.position));
						anim.CrossFade("Run", 0);
					}

					break;
				case EnemyArmyManager.GlobalOrders.ATTACK:


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

						if (stateArmy != StateArmy.ATTACKING)
						{
							lastAttackTime = Time.time;
						}
					}
					else
					{
						stateArmy = StateArmy.IDLE;

						anim.CrossFade("Run", 0);

					}

					if (stateArmy == StateArmy.ATTACKING && !isDoingAttack)
					{
						if ((Time.time - lastAttackTime) > delayBetweenAttacks)
						{
							Attack((currentTarget.transform.position - this.transform.position).normalized);
						}
					}


					break;
			}


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

		StartCoroutine(DoAttack(currentTarget));



	}

	private IEnumerator DoAttack(EnemyAttackPoints target = null)
	{
		yield return new WaitForSeconds(0.2f);

		if (target != null)
		{
			target.GetDamage();
		}



		yield return new WaitForSeconds(0.25f);
		anim.CrossFade("Idle", 0);
		lastAttackTime = Time.time;

		yield return new WaitForSeconds(0.2f);
		isDoingAttack = false;

	}
}
