using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyArmyManager : MonoBehaviour
{
	public enum GlobalOrders { RETREAT, DEFENSE, ATTACK };

	public GlobalOrders globalOrder;
	public List<EnemyDefensePoint> defensePoint;
	public List<EnemyAttackPoints> attackPoints;
	public bool autoAssign = true;

	public static EnemyArmyManager Instance { get; private set; }


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		// Si no existe una instancia, la asignamos
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject); // Opcional, si quieres que sobreviva entre escenas
		}
		else
		{
			Destroy(gameObject); // Si ya existe una instancia, destruimos la duplicada
		}
	}

	// Update is called once per frame
	void Update()
	{

		EnemyAttackPoints[] attackPoint = FindObjectsByType<EnemyAttackPoints>(FindObjectsSortMode.None)
					 .Where(t => t.gameObject.activeSelf)
					 .ToArray();
		attackPoints.Clear();

		foreach(EnemyAttackPoints point in attackPoint) 
		{ 
			attackPoints.Add(point);
		}

		if (autoAssign && globalOrder == GlobalOrders.ATTACK)
		{
			AssignTroops();
		}


	}
	void AssignTroops()
	{
		EnemyTroops[] allTroops = FindObjectsByType<EnemyTroops>(FindObjectsSortMode.None)
							 .Where(t => t.gameObject.activeSelf)
							 .ToArray();

		foreach (EnemyTroops troop in allTroops)
		{
			// Tropas sin target
			if (!troop.isAttacking)
			{
				EnemyAttackPoints closest = GetClosestAvailableAttackPoint(troop.transform.position);
				if (closest != null)
				{
					troop.SetTarget(closest);
				}
			}
			else
			{
				// Tropas con target: comprobar si hay un hueco más cercano
				EnemyAttackPoints newClosest = GetClosestAvailableAttackPoint(troop.transform.position, troop.currentTarget);
				if (newClosest != null)
				{
					float distToCurrent = (troop.currentTarget.transform.position - troop.transform.position).sqrMagnitude;
					float distToNew = (newClosest.transform.position - troop.transform.position).sqrMagnitude;

					if (distToNew < distToCurrent) // solo reasignar si está más cerca
					{
						troop.ClearTarget();
						troop.SetTarget(newClosest);
					}
				}
			}
		}
	}
	EnemyAttackPoints GetClosestAvailableAttackPoint(Vector3 troopPos, EnemyAttackPoints ignoreTarget = null)
	{
		EnemyAttackPoints closest = null;
		float minSqr = float.MaxValue;

		foreach (var point in attackPoints)
		{
			if (point == ignoreTarget) continue;

			if (!point.HasSpace) continue;

			float sqrDist = (point.transform.position - troopPos).sqrMagnitude;
			if (sqrDist < minSqr)
			{
				minSqr = sqrDist;
				closest = point;
			}
		}

		return closest;
	}
	public Vector3 DefensePointUnoccuped(Vector3 pos)
	{
		float minSqrDistance = float.MaxValue;

		for (int i = 0; i < defensePoint.Count; i++)
		{
			if (defensePoint[i].occuped)
				continue; // Ignora puntos ocupados

			return defensePoint[i].transform.position;
		}



		// Si no hay ningún punto libre, devuelve la posición original o Vector3.zero
		return pos;
	}


	public void SetGlobalOrderRetreat()
	{
		if (this.globalOrder == GlobalOrders.RETREAT)
			return;

		this.globalOrder = GlobalOrders.RETREAT;

		EnemyTroops[] allTroops = FindObjectsByType<EnemyTroops>(FindObjectsSortMode.None)
			 .Where(t => t.gameObject.activeSelf)
			 .ToArray();

		foreach (EnemyTroops troop in allTroops)
		{
			troop.ClearTarget();
		}
	}
	public void SetGlobalOrderDefense()
	{
		if (this.globalOrder == GlobalOrders.DEFENSE)
			return;

		this.globalOrder = GlobalOrders.DEFENSE;

		EnemyTroops[] allTroops = FindObjectsByType<EnemyTroops>(FindObjectsSortMode.None)
					 .Where(t => t.gameObject.activeSelf)
					 .ToArray();

		foreach (EnemyTroops troop in allTroops)
		{
			troop.ClearTarget();

		}
	}
	public void SetGlobalOrderAttack()
	{
		if (this.globalOrder == GlobalOrders.ATTACK)
			return;

		this.globalOrder = GlobalOrders.ATTACK;

	}
}
