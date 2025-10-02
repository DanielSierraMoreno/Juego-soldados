using System.Collections;
using UnityEngine;

public class EnemyCastle : MonoBehaviour
{
	public EnemyWaveData waveData; // Asignar desde inspector

	public static EnemyCastle Instance { get; private set; }

	public Transform pos;

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

		if (waveData != null)
			StartCoroutine(RunWave());
	}

	// Update is called once per frame
	void Update()
    {
        
    }

	private IEnumerator RunWave()
	{
		foreach (var phase in waveData.phases)
		{
			for (int r = 0; r < phase.repeat; r++)
			{
				foreach (var enemy in phase.enemies)
				{
					for (int i = 0; i < enemy.count; i++)
					{
						SpawnEnemy(enemy.type);
					}

				}
				yield return new WaitForSeconds(phase.spawnInterval);

			}
		}
	}
	private void SpawnEnemy(EnemyType type)
	{
		switch (type)
		{
			case EnemyType.Torch:
				SpawnTorch();
				break;
			case EnemyType.Dinamite:
				SpawnDinamite();
				break;
			case EnemyType.Suicide:
				SpawnSuicide();
				break;
				// Agrega más casos si hay otros tipos
		}
	}
	public void SpawnTorch()
	{
		foreach (TorchEnemy warrior in FindObjectsOfType<TorchEnemy>(true))
		{
			if (!warrior.gameObject.activeSelf)
			{
				warrior.transform.localPosition = Vector3.zero;
				warrior.gameObject.SetActive(true);
				warrior.Reset();

				return;
			}
		}
	
	}

	public void SpawnDinamite()
	{
		foreach (DinamiteEnemy warrior in FindObjectsOfType<DinamiteEnemy>(true))
		{
			if (!warrior.gameObject.activeSelf)
			{
				warrior.transform.localPosition = Vector3.zero;
				warrior.gameObject.SetActive(true);
				warrior.Reset();

				return;
			}
		}

	}

	public void SpawnSuicide()
	{
		foreach (SuicideEnemy warrior in FindObjectsOfType<SuicideEnemy>(true))
		{
			if (!warrior.gameObject.activeSelf)
			{
				warrior.transform.localPosition = Vector3.zero;
				warrior.gameObject.SetActive(true);
				warrior.Reset();

				return;
			}
		}

	}
	private IEnumerator Spawn(EnemyAttackPoints target = null)
	{
		yield return new WaitForSeconds(5);

		SpawnTorch();
		StartCoroutine(Spawn());

	}
}
