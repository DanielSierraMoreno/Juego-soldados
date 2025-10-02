using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAttackPoints : MonoBehaviour
{
	public List<EnemyTroops> assignedTroops = new List<EnemyTroops>();
	public int maxTroops = 5;

	public bool HasSpace => assignedTroops.Count < maxTroops;
	
	public int lives = 5;

	int maxLives;


	public Slider structureLives;
	float timeToShowSliderLive = -100;
	// Start is called once before the first execution of Update after the MonoBehaviour is created

	private void Start()
	{
		Initialize();
	}
	public void Initialize()
	{
		maxLives = lives;
	}
	public void ResetLives()
	{
		lives = maxLives;
	}

	// Update is called once per frame
	void Update()
	{
		if (structureLives == null)
			return;

		if ((Time.time - timeToShowSliderLive) < 2)
		{
			structureLives.value = (float)lives / (float)maxLives;

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

			EnemyTroops a = assignedTroops[0];
			
			assignedTroops[0]?.ClearTarget();
			assignedTroops.Remove(a);

		}
		EnemyArmyManager.Instance.attackPoints.Remove(this);

		Mine mine = GetComponentInChildren<Mine>();
		if (mine != null)
		{
			mine.isDestroyed = true;
			this.enabled = false;

			if (mine.mining)
			{
				mine.miningStartTime = Time.time;
			}
		}
		else
			this.gameObject.SetActive(false);
	}
}
