using UnityEngine;
using UnityEngine.AI;

public class Pawn : Troop
{
    public enum State { MOVING, MINING, CARRYNG, IDLE};
    public State state;

	public bool haveObject = false;

	public NavMeshAgent agent;

	public Vector3 targetDestination;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	protected override void Start()
	{
		base.Start();
		state = State.IDLE;
		troopType = Type.PAWN;

		agent.updateRotation = false;
		agent.updateUpAxis = false;

	}

	// Update is called once per frame
	protected override void Update()
    {
        if (!selected)
		{

			if (targetDestination.x - transform.position.x > 0)
			{
				this.transform.localScale = new Vector3(1, 1, 1);
			}
			else if (targetDestination.x - transform.position.x < 0)
			{
				this.transform.localScale = new Vector3(-1, 1, 1);
			}



			if (state == State.IDLE && !haveObject)
			{
				agent.destination = MineController.Instance.GetMineUnocupped(this.transform.position);
				state = State.MOVING;
				targetDestination = agent.destination;
				anim.CrossFade(haveObject ? "Run_0" : "Run", 0);
			}
			else if (state == State.MOVING && !haveObject)
			{
				agent.destination = MineController.Instance.GetMineUnocupped(this.transform.position);
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

			anim.CrossFade(haveObject ? "Run_0":"Run", 0);
		}
		else if (state == State.MOVING)
		{
			state = State.IDLE;

			anim.CrossFade(haveObject ? "Idle_0" : "Idle", 0);

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
}
