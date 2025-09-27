using UnityEngine;

public class Mine : MonoBehaviour
{
	public Pawn player;
	public bool mining = false;

	public float miningTime = 5;

	float miningStartTime = 0;

	public bool isPlayer
	{
		get { return player == PlayerController.Instance.currentPlayer; }
	}    
	
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void FixedUpdate()
	{
		if(mining)
		{
			if(player.state != Pawn.State.MINING)
			{
				mining = false;
			}

			if((Time.time- miningStartTime) > miningTime)
			{
				mining = false;
				player.haveObject = true;
				player.agent.isStopped = false;

				player.state = Pawn.State.IDLE;
				player.anim.CrossFade("Idle_0", 0);
				player.transform.GetChild(0).GetChild(1).GetComponent<Animator>().CrossFade("Spawn", 0);

			}
		}
	}
	private void OnTriggerStay2D(Collider2D collision)
	{
		if(collision.GetComponent<Pawn>() != null && !mining)
        {
			player = collision.GetComponent<Pawn>();


			if(!mining && !isPlayer && !player.haveObject)
			{
				player.agent.isStopped = true;
				StartMining();
			}
							
        }
	}
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.GetComponent<Pawn>() != null && !mining)
		{
			if (collision.GetComponent<Pawn>() == PlayerController.Instance.currentPlayer)
			{
				player = null;

			}
		}
	}

	public void StartMining()
	{
		mining = true;
		player.state = Pawn.State.MINING;
		player.anim.CrossFade("Build", 0);
		miningStartTime = Time.time;
	}
}
