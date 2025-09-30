using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Pawn : Troop
{
    public enum State { MOVING, MINING, CARRYNG, IDLE};
    public State state;

	public bool haveObject = false;


	public Vector3 targetDestination;
	public Slider slider;

	public int money = 0;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	protected override void Start()
	{
		base.Start();
		state = State.IDLE;
		troopType = Type.PAWN;
		agent.speed = moveSpeed;

		agent.updateRotation = false;
		agent.updateUpAxis = false;

	}

	// Update is called once per frame
	protected override void Update()
    {
		if(state == State.MINING)
			slider.gameObject.SetActive(true);
		else
			slider.gameObject.SetActive(false);

        if (!selected)
		{

			if (targetDestination.x - transform.position.x > 0)
			{
				this.transform.GetChild(0).localScale = new Vector3(1, 1, 1);
			}
			else if (targetDestination.x - transform.position.x < 0)
			{
				this.transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
			}



			if (state == State.IDLE && !haveObject)
			{
				Vector3 a = MineController.Instance.GetMineUnocupped(this.transform.position);
				agent.SetDestination(a);
				state = State.MOVING;
				targetDestination = agent.destination;
				anim.CrossFade(haveObject ? "Run_0" : "Run", 0);
			}
			else if (state == State.MOVING && !haveObject)
			{
				agent.SetDestination(MineController.Instance.GetMineUnocupped(this.transform.position));
				targetDestination = agent.destination;
			}
			else if(state == State.IDLE && haveObject)
			{
				agent.destination = Castle.Instance.pos.position;
				state = State.MOVING;
				targetDestination = agent.destination;
				anim.CrossFade(haveObject ? "Run_0" : "Run", 0);
			}
			else if (state == State.MOVING && !haveObject)
			{
				agent.destination = Castle.Instance.pos.position;
				targetDestination = agent.destination;
			}



		}
    }

	public override void Move(Vector2 dir)
	{
		if (dir.magnitude > 0)
		{
			state = State.MOVING;
			agent.isStopped = false;

			anim.CrossFade(haveObject ? "Run_0":"Run", 0);
		}
		else if (state == State.MOVING)
		{
			state = State.IDLE;

			anim.CrossFade(haveObject ? "Idle_0" : "Idle", 0);

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
		agent.SetDestination(targetPosition);
	}


	public void Reset()
	{
		haveObject = false;
		state = State.IDLE;
		money = 0;
	}

	public override void DeSelect()
	{
		selected = false;

		agent.isStopped = state == State.MINING;
	}
	public override void Select()
	{
		selected = true;

		agent.isStopped = true; 

	}
	public override void SetDefense()
	{

	}
}
