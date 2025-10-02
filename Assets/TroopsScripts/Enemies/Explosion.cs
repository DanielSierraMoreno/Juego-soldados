using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Explosion : MonoBehaviour
{
	public float radius = 1f;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
        Destroy(this.gameObject,1);
		Collider2D[] hits = Physics2D.OverlapCircleAll(this.transform.position, radius);


		foreach (var hit in hits)
		{
			// Solo nos interesa enemigos con AttackPoints
			EnemyAttackPoints ap = hit.GetComponent<EnemyAttackPoints>();
			if (ap != null)
			{
				ap.GetDamage(2);
			}
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }


}
