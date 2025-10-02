using System.Collections;
using TMPro;
using UnityEngine;

public class Archer : Troop
{

	public float attackRange = 1.5f;

	public GameObject arrowPrefab;
	public Transform arrowSpawn;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	protected override void Start()
	{
		base.Start();
		stateArmy = StateArmy.IDLE;
		troopType = Type.ARCHER;

		if (!isTower )
		{
			agent.speed = moveSpeed;
			agent.updateRotation = false;
			agent.updateUpAxis = false;
			this.GetComponent<EnemyAttackPointIsTroop>().lives = maxLive;
		}


	}

	// Update is called once per frame
	protected override void Update()
	{
		base.Update();

		if (isTower)
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


			switch (ArmyManager.Instance.globalOrder)
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
			StartCoroutine(DoAttack(null, dir));


	}

	private IEnumerator DoAttack(AttackPoints target = null, Vector3 direction = default)
	{
		yield return new WaitForSeconds(0.3f);

		if (target != null)
		{
			if (selected)
				yield return null;

			GameObject arrow = Instantiate(arrowPrefab, arrowSpawn.position, arrowSpawn.rotation);
			Arrow arrowScript = arrow.GetComponent<Arrow>();
			if (arrowScript != null)
			{
				arrowScript.Shoot(target); // pasar posición del target
			}
		}
		else
		{
			AttackPoints manualTarget = GetClosestAttackPointInRect(direction);

			if (manualTarget != null)
			{
				GameObject arrow = Instantiate(arrowPrefab, arrowSpawn.position, arrowSpawn.rotation);
				Arrow arrowScript = arrow.GetComponent<Arrow>();
				if (arrowScript != null)
				{
					arrowScript.Shoot(manualTarget);
				}
			}
			else
			{
				GameObject arrow = Instantiate(arrowPrefab, arrowSpawn.position, arrowSpawn.rotation);
				Arrow arrowScript = arrow.GetComponent<Arrow>();
				if (arrowScript != null)
				{
					Vector2 center = transform.position;

					Vector2 direction2 = new Vector2(direction.x, direction.y).normalized; // derecha del personaje
					center += direction2 * (attackRange);

					arrowScript.Shoot(center);
				}
			}
		}


		yield return new WaitForSeconds(0.35f);
		anim.CrossFade("Idle", 0);
		lastAttackTime = Time.time;

		yield return new WaitForSeconds(0.2f);
		isDoingAttack = false;

	}

	private AttackPoints GetClosestAttackPointInRect(Vector3 dir)
	{
		float width = 3f; // ancho fijo
		float height = attackRange; // largo del rectángulo
		Vector2 center = transform.position;

		// Queremos que el rectángulo "apunte" hacia donde mira el personaje
		Vector2 direction = new Vector2(dir.x, dir.y).normalized; // derecha del personaje
		center += direction * (attackRange / 2f); // centro del rectángulo desplazado hacia delante

		// OverlapBoxAll pide halfSize
		Vector2 size = new Vector2(attackRange, width);
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

		Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, angle);

		AttackPoints closest = null;
		float closestDist = Mathf.Infinity;

		foreach (var hit in hits)
		{
			AttackPoints ap = hit.GetComponent<AttackPoints>();
			if (ap != null)
			{
				float dist = Vector2.Distance(transform.position, ap.transform.position);
				if (dist < closestDist)
				{
					closestDist = dist;
					closest = ap;
				}
			}
		}

		return closest;
	}

}
