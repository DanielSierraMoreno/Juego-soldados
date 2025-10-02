using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyTroops : MonoBehaviour
{
	public enum Type { TORCH, DINAMITE, SUICIDE };

	public float moveSpeed = 5;

	public Animator anim;

	public Type troopType;

	public NavMeshAgent agent;

	public enum StateArmy { MOVING, IDLE, ATTACKING, DEFENSE };
	public StateArmy stateArmy;

	public bool targetAttack = false;

	public EnemyAttackPoints currentTarget;
	public bool isAttacking = false;
	public bool isDoingAttack = false;

	protected float lastAttackTime = 0;

	public float delayBetweenAttacks = 0.5f;

	public int maxLive = 5;
	public Slider sliderLive;
	public float timeToShowSliderLive = -100;

	public float visionRangeOnDefense = 8;
	public bool isTower = false;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	protected virtual void Start()
	{
		timeToShowSliderLive = -100;
	}

	// Update is called once per frame
	protected virtual void Update()
	{
		if ((Time.time - timeToShowSliderLive) < 2)
		{
			sliderLive.value = (float)this.GetComponent<AttackPointIsEnemy>().lives / (float)maxLive;

			// tiempo relativo dentro del intervalo de 2s
			float t = (Time.time - timeToShowSliderLive) / 2f;

			// alpha va de 1 a 0 en esos 2s
			sliderLive.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, t);
		}
		else
		{
			sliderLive.GetComponent<CanvasGroup>().alpha = 0;
		}


	}

	public void SetTarget(EnemyAttackPoints target)
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
		if (dir.magnitude > 0)
		{
			anim.SetBool("Move", true);
		}
		else
		{
			anim.SetBool("Move", false);

		}

		if (dir.x > 0)
		{
			this.transform.localScale = new Vector3(1, 1, 1);
		}
		else if (dir.x < 0)
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
