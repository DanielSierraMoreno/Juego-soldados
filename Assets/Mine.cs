using UnityEngine;

public class Mine : MonoBehaviour
{
	public Pawn player;
	public bool mining = false;

	public float miningTime = 5;

	float miningStartTime = 0;

	public bool isPlayer
	{
		get { 

			if (PlayerController.Instance.currentPlayer != null)
			return (player == PlayerController.Instance.currentPlayer);

			return false;
		
		}
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
				player.transform.GetChild(0).GetChild(0).GetComponent<Animator>().CrossFade("Spawn", 0);

				if (isPlayer)
				{
					player.money = 125;
					player.agent.isStopped = true;
				}
				else
				{
					player.agent.isStopped = false;

					player.money = 75;
				}
			}

			if (isPlayer)
			{
				player.slider.value = ((Time.time - miningStartTime)/ miningTime);

			}
			else
			{
				player.slider.value = ((Time.time - miningStartTime) / miningTime)*0.75f;
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
			if (collision.GetComponent<Pawn>() == player)
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
