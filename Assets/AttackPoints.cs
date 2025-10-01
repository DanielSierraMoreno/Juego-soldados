using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackPoints : MonoBehaviour
{
	public List<Troop> assignedTroops = new List<Troop>();
	public int maxTroops = 5;

	public bool HasSpace => assignedTroops.Count < maxTroops;
	public int lives = 5;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

	// Update is called once per frame
	void Update()
	{
		
	}

	public virtual void GetDamage()
	{
		lives--;

		if (lives <= 0)
		{
			Destroy();
		}
	}

	public virtual void Destroy()
	{
		for (int i = 0; assignedTroops.Count != 0; i = 0)
		{
			assignedTroops[0].ClearTarget();
		}

		ArmyManager.Instance.attackPoints.Remove(this);
		this.gameObject.SetActive(false);

	}
}
