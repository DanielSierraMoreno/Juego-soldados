using UnityEngine;

public class AttackPointIsEnemy : AttackPoints
{
    EnemyTroops troop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        troop = GetComponent<EnemyTroops>();
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
