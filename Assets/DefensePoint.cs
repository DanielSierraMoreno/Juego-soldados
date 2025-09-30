using UnityEngine;

public class DefensePoint : MonoBehaviour
{

    public bool occuped = false;
	public bool canMove = false;

	Troop troop = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(occuped && !canMove)
        {
            troop?.agent.SetDestination(this.transform.position);
            troop.agent.stoppingDistance = 0;

            if (Vector3.Distance(this.transform.position, troop.transform.position) < 0.05f)
            {
                canMove = true;
                troop.agent.isStopped = true;
				troop.transform.GetChild(0).localScale = new Vector3(1, 1, 1);
				troop.anim.CrossFade("Idle", 0);
			}
		}
    }

	private void OnTriggerStay2D(Collider2D other)
	{
        Troop troop = other.GetComponent<Troop>();
		if(troop != null && ArmyManager.Instance.globalOrder == ArmyManager.GlobalOrders.DEFENSE && !occuped)
        {
            if(troop.troopType != Troop.Type.PAWN)
            {				
                if (Vector3.Distance(troop.agent.destination, this.transform.position) < 0.05f)
                {
                    occuped = true;
				    this.troop = troop;
				    troop.SetDefense();
                }



			}
        }
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		Troop troop = collision.GetComponent<Troop>();
		if (troop != null && occuped)
		{
			if (troop.troopType != Troop.Type.PAWN)
			{
                if (troop == this.troop)
				{
					troop.SetStopDefense();

					canMove = false;
					occuped = false;
				    troop = null;
                }

			}
		}
	}
}
