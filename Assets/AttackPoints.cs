using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackPoints : MonoBehaviour
{
	public List<Troop> assignedTroops = new List<Troop>();
	public int maxTroops = 5;

	public bool HasSpace => assignedTroops.Count < maxTroops;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

	// Update is called once per frame
	void Update()
	{
		
	}


	public void Destroy()
	{
		foreach (var troop in assignedTroops)
		{
			troop.ClearTarget();
		}
	}
}
