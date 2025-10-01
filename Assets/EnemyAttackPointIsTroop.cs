using UnityEngine;

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
	public override void GetDamage()
	{
		base.GetDamage();
	}
	public override void Destroy()
	{
		base.Destroy();
	}
}
