using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using TMPro;

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
		this.GetComponent<EnemyAttackPointIsTroop>().lives = maxLive;

	}

	// Update is called once per frame
	protected override void Update()
	{
		base.Update();

		if (!selected)
		{
			if (isDoingAttack) { return; }




			if (stateArmy != StateArmy.DEFENSE)
			{
				Vector3 vel = agent.velocity;

				if (vel.sqrMagnitude > 0.01f)
				{
					if (vel.x > 0)
						this.transform.GetChild(0).localScale = new Vector3(1, 1, 1);
					else if (vel.x < 0)
						this.transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
				}
			}


			switch(ArmyManager.Instance.globalOrder)
			{
				case ArmyManager.GlobalOrders.RETREAT:

					break;
				case ArmyManager.GlobalOrders.DEFENSE:

					agent.radius = 0.1f;



					if (isAttacking)
					{
						Vector2 pos = transform.position;

						// Detectamos todos los colliders en el radio
						Collider2D[] hits = Physics2D.OverlapCircleAll(pos, visionRangeOnDefense);

						AttackPoints closest = null;
						float closestDist = Mathf.Infinity;

						foreach (var hit in hits)
						{
							// Solo nos interesa enemigos con AttackPoints
							AttackPoints ap = hit.GetComponent<AttackPoints>();
							if (ap != null)
							{
								float dist = Vector2.Distance(pos, ap.transform.position);

								if (ap.HasSpace)
								{
									// Si hay espacio, comprobamos si es el más cercano
									if (dist < closestDist)
									{
										closest = ap;
										closestDist = dist;
									}
								}
								else
								{
									if (ap.assignedTroops.Contains(this))
									{
										if (dist < closestDist)
										{
											closest = ap;
											closestDist = dist;
										}
									}

								}
							}
						}

						if (closest != null)
						{
							if(!closest.assignedTroops.Contains(this))
							{
								SetTarget(closest);
							}

						}
						else
						{
							ClearTarget();
							stateArmy = StateArmy.MOVING;
							break;
						}


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
						return;
					}


					if (stateArmy != StateArmy.DEFENSE)
					{
						agent.isStopped = false;

						agent.SetDestination(ArmyManager.Instance.DefensePointUnoccuped(this.transform.position));
						anim.CrossFade("Run", 0);
					}
					else
					{
							Vector2 pos = transform.position;

							// Detectamos todos los colliders en el radio
							Collider2D[] hits = Physics2D.OverlapCircleAll(pos, visionRangeOnDefense);

							AttackPoints closest = null;
							float closestDist = Mathf.Infinity;

							foreach (var hit in hits)
							{
								// Solo nos interesa enemigos con AttackPoints
								AttackPoints ap = hit.GetComponent<AttackPoints>();
								if (ap != null)
								{
									float dist = Vector2.Distance(pos, ap.transform.position);

									if (ap.HasSpace)
									{
										// Si hay espacio, comprobamos si es el más cercano
										if (dist < closestDist)
										{
											closest = ap;
											closestDist = dist;
										}
									}
									else
									{
									if (ap.assignedTroops.Contains(this))
									{
										if (dist < closestDist)
										{
											closest = ap;
											closestDist = dist;
										}
									}
								}
								}
							}

							if (closest != null)
							{
								if (!closest.assignedTroops.Contains(this))
								{
									ClearTarget();
									SetTarget(closest);
									agent.isStopped = false;

								}
							}
					}






					break;
				case ArmyManager.GlobalOrders.ATTACK:

					if (currentTarget == null)
						break;
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

		if (this.gameObject.activeSelf) 
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
		targetAttack = false;

		currentTarget = null;
		isAttacking = false;
		isDoingAttack = false;
		this.GetComponent<EnemyAttackPointIsTroop>().lives = maxLive;
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
		yield return new WaitForSeconds(0.3f);

        if (!this.gameObject.activeSelf)
        {
			yield return null;
        }

        if (target != null) 
		{ 		
			target.GetDamage();
		}
		else
		{
			float radius = 2f;
			Vector2 pos = transform.position;

			AttackPoints closest = null;
			float closestDist = Mathf.Infinity;

			Collider2D[] hits = Physics2D.OverlapCircleAll(pos, radius);
			foreach (var hit in hits)
			{
				AttackPoints ap = hit.GetComponent<AttackPoints>();
				if (ap != null)
				{
					Vector2 dir = (Vector2)ap.transform.position - pos;
					float dist = dir.magnitude;

					if (dist < closestDist) // derecha + más cerca
					{
						if (this.transform.GetChild(0).localScale.x == -1)
						{
							if (ap.transform.position.x < this.transform.position.x+0.2f)
							{
								closest = ap;
								closestDist = dist;
							}
						}
						else
						{
							if (ap.transform.position.x > this.transform.position.x - 0.2f)
							{
								closest = ap;
								closestDist = dist;
							}
						}

					}
				}
			}

			if(closest != null )
			{
				closest.GetDamage();

			}

		}



		yield return new WaitForSeconds(0.15f);
		anim.CrossFade("Idle", 0);
		lastAttackTime = Time.time;

		yield return new WaitForSeconds(0.2f);
		isDoingAttack = false;

	}

}
