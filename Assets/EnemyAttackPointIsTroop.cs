using UnityEngine;
using UnityEngine.UI;

public class EnemyAttackPointIsTroop : EnemyAttackPoints
{
	Troop troop;



	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		troop = GetComponent<Troop>();
	}

	// Update is called once per frame
	void Update()
	{

	}
	public override void GetDamage(int damage = 1)
	{
		base.GetDamage(damage);
		troop.timeToShowSliderLive = Time.time;
	}

	public void Select()
	{
		troop.timeToShowSliderLive = Time.time;

	}
	public override void Destroy()
	{
		base.Destroy();

		if (troop.currentTarget != null)
		{
			troop.currentTarget.assignedTroops.Remove(troop);
		}

        if (troop.selected)
        {
			troop.DeSelect();
        }

		
    }
}
