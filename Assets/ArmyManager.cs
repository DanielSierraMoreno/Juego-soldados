using UnityEngine;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;

public class ArmyManager : MonoBehaviour
{
    public enum GlobalOrders {RETREAT, DEFENSE, ATTACK};

	public GlobalOrders globalOrder;
	public List<DefensePoint> defensePoint;
	public List<AttackPoints> attackPoints;
	public bool autoAssign = true;

	public static ArmyManager Instance { get; private set; }


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
		AttackPoints[] attackPoint = FindObjectsByType<AttackPoints>(FindObjectsSortMode.None)
					 .Where(t => t.gameObject.activeSelf)
					 .ToArray();
		attackPoints.Clear();

		foreach (AttackPoints point in attackPoint)
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
		Troop[] allTroops = FindObjectsByType<Troop>(FindObjectsSortMode.None)
							 .Where(t => t.gameObject.activeSelf)
							 .Where(t => !t.selected)
							 .Where(t => t.troopType != Troop.Type.PAWN).ToArray();

		foreach (Troop troop in allTroops)
		{
			// Tropas sin target
			if (!troop.isAttacking)
			{
				AttackPoints closest = GetClosestAvailableAttackPoint(troop.transform.position);
				if (closest != null)
				{
					troop.SetTarget(closest);
				}
			}
			else
			{
				// Tropas con target: comprobar si hay un hueco más cercano
				AttackPoints newClosest = GetClosestAvailableAttackPoint(troop.transform.position, troop.currentTarget);
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
	AttackPoints GetClosestAvailableAttackPoint(Vector3 troopPos, AttackPoints ignoreTarget = null)
	{
		AttackPoints closest = null;
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

		Troop[] allTroops = FindObjectsByType<Troop>(FindObjectsSortMode.None)
			 .Where(t => t.gameObject.activeSelf)
			 .Where(t => !t.selected)
			 .Where(t => t.troopType != Troop.Type.PAWN).ToArray();

		foreach (Troop troop in allTroops)
		{
			troop.ClearTarget();
		}
	}
	public void SetGlobalOrderDefense()
	{
		if (this.globalOrder == GlobalOrders.DEFENSE)
			return;

		this.globalOrder = GlobalOrders.DEFENSE;

		Troop[] allTroops = FindObjectsByType<Troop>(FindObjectsSortMode.None)
					 .Where(t => t.gameObject.activeSelf)
					 .Where(t => !t.selected)
					 .Where(t => t.troopType != Troop.Type.PAWN).ToArray();

		foreach (Troop troop in allTroops)
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
