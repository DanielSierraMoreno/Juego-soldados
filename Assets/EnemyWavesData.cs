using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaveData", menuName = "Enemy/WaveData")]
public class EnemyWaveData : ScriptableObject
{
	[System.Serializable]
	public class EnemyEntry
	{
		public EnemyType type;   // Tipo de enemigo (enum)
		public int count = 1;    // Cu�ntos spawnear
	}

	[System.Serializable]
	public class Phase
	{
		public EnemyEntry[] enemies;  // Enemigos de esta fase
		public float spawnInterval = 1f; // Tiempo entre spawns
		public int repeat = 1;        // Cu�ntas veces repetir esta fase antes de pasar a la siguiente
	}

	public Phase[] phases;
}

public enum EnemyType
{
	Torch,
	Dinamite,
	Suicide
	// Agrega m�s tipos seg�n tu juego
}
