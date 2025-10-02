using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class DinamiteEnemy : EnemyTroops
{

	public float attackRange = 1.5f;

	public GameObject dinamitePrefab;
	public Transform dinamiteSpawn;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	protected override void Start()
	{
		base.Start();
		stateArmy = StateArmy.IDLE;
		troopType = Type.DINAMITE;

		if (!isTower)
		{
			agent.speed = moveSpeed;
			agent.updateRotation = false;
			agent.updateUpAxis = false;
			this.GetComponent<AttackPointIsEnemy>().lives = maxLive;
		}
	}

	// Update is called once per frame
	protected override void Update()
	{
		base.Update();
		if (isDoingAttack) { return; }

		if (isTower)
		{
			Vector2 pos = transform.position;

			// Detectamos todos los colliders en el radio
			Collider2D[] hits = Physics2D.OverlapCircleAll(pos, visionRangeOnDefense);

			EnemyAttackPoints closest = null;
			float closestDist = Mathf.Infinity;
			foreach (var hit in hits)
			{
				// Solo nos interesa enemigos con AttackPoints
				EnemyAttackPoints ap = hit.GetComponent<EnemyAttackPoints>();
				if (ap != null)
				{
					float dist = Vector2.Distance(pos, ap.transform.position);


					// Si hay espacio, comprobamos si es el más cercano
					if (dist < closestDist)
					{
						closest = ap;
						closestDist = dist;
					}


				}
			}

			if (currentTarget != closest)
			{
				lastAttackTime = Time.time;
				currentTarget = closest;
			}


			if (closest != null)
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
			Vector3 vel = agent.velocity;

			if (vel.sqrMagnitude > 0.01f)
			{
				if (vel.x > 0)
					this.transform.GetChild(0).localScale = new Vector3(1, 1, 1);
				else if (vel.x < 0)
					this.transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
			}
		}


		switch (EnemyArmyManager.Instance.globalOrder)
		{
			case EnemyArmyManager.GlobalOrders.RETREAT:

				break;
			case EnemyArmyManager.GlobalOrders.DEFENSE:

				agent.radius = 0.1f;
				if (isAttacking)
				{
					Vector2 pos = transform.position;

					// Detectamos todos los colliders en el radio
					Collider2D[] hits = Physics2D.OverlapCircleAll(pos, visionRangeOnDefense);

					EnemyAttackPoints closest = null;
					float closestDist = Mathf.Infinity;

					foreach (var hit in hits)
					{
						// Solo nos interesa enemigos con AttackPoints
						EnemyAttackPoints ap = hit.GetComponent<EnemyAttackPoints>();
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

					agent.SetDestination(EnemyArmyManager.Instance.DefensePointUnoccuped(this.transform.position));
					anim.CrossFade("Run", 0);
				}
				else
				{
					Vector2 pos = transform.position;

					// Detectamos todos los colliders en el radio
					Collider2D[] hits = Physics2D.OverlapCircleAll(pos, visionRangeOnDefense);

					EnemyAttackPoints closest = null;
					float closestDist = Mathf.Infinity;

					foreach (var hit in hits)
					{
						// Solo nos interesa enemigos con AttackPoints
						EnemyAttackPoints ap = hit.GetComponent<EnemyAttackPoints>();
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
		targetAttack = false;

		currentTarget = null;
		isAttacking = false;
		isDoingAttack = false;
		this.GetComponent<AttackPointIsEnemy>().lives = maxLive;
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
			GameObject dinamite = Instantiate(dinamitePrefab, dinamiteSpawn.position, dinamiteSpawn.rotation);
			Dinamite dinamiteScript = dinamite.GetComponent<Dinamite>();
			if (dinamiteScript != null)
			{
				dinamiteScript.Shoot(target); // pasar posición del target
			}
		}



		yield return new WaitForSeconds(0.25f);
		anim.CrossFade("Idle", 0);
		lastAttackTime = Time.time;

		yield return new WaitForSeconds(0.2f);
		isDoingAttack = false;

	}
}
