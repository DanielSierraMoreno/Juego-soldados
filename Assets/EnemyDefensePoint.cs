using UnityEngine;

public class EnemyDefensePoint : MonoBehaviour
{

	public bool occuped = false;
	public bool canMove = false;

	EnemyTroops troop = null;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (occuped && !canMove)
		{
			troop?.agent.SetDestination(this.transform.position);
			troop.agent.stoppingDistance = 0;

			if (Vector3.Distance(this.transform.position, troop.transform.position) < 0.05f)
			{
				canMove = true;
				troop.agent.isStopped = true;
				troop.transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
				troop.anim.CrossFade("Idle", 0);
			}
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		EnemyTroops troop = other.GetComponent<EnemyTroops>();
		if (troop != null && EnemyArmyManager.Instance.globalOrder == EnemyArmyManager.GlobalOrders.DEFENSE && !occuped)
		{
			if (Vector3.Distance(troop.agent.destination, this.transform.position) < 0.05f)
			{
				occuped = true;
				this.troop = troop;
				troop.SetDefense();
			}
		}

	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		EnemyTroops troop = collision.GetComponent<EnemyTroops>();
		if (troop != null && occuped)
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
