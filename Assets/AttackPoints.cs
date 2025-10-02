using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AttackPoints : MonoBehaviour
{
	public List<Troop> assignedTroops = new List<Troop>();
	public int maxTroops = 5;

	public bool HasSpace => assignedTroops.Count < maxTroops;
	int maxLive = 5;

	public int lives = 5;

	public Slider structureLives;
	float timeToShowSliderLive = -100;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		maxLive = lives;
	}

	// Update is called once per frame
	void Update()
	{
		if (structureLives == null)
			return;
		if ((Time.time - timeToShowSliderLive) < 2)
		{
			structureLives.value = (float)lives / (float)maxLive;

			// tiempo relativo dentro del intervalo de 2s
			float t = (Time.time - timeToShowSliderLive) / 2f;

			// alpha va de 1 a 0 en esos 2s
			structureLives.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, t);
		}
		else
		{
			structureLives.GetComponent<CanvasGroup>().alpha = 0;
		}
	}

	public virtual void GetDamage(int damage = 1)
	{
		lives -= damage;

		if (lives <= 0)
		{
			Destroy();
		}

		if (structureLives != null)
		{
			timeToShowSliderLive = Time.time;
		}
	}

	public virtual void Destroy()
	{
		if (!this.gameObject.activeSelf)
			return;


		for (int i = 0; assignedTroops.Count != 0; i = 0)
		{
			Troop a = assignedTroops[0];
			assignedTroops[0]?.ClearTarget();
			assignedTroops.Remove(a);
		}

		ArmyManager.Instance.attackPoints.Remove(this);
		this.gameObject.SetActive(false);

	}
}
