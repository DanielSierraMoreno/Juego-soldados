using UnityEngine;

public class Dinamite : MonoBehaviour
{
	public float speed = 10f;       // velocidad constante
	public float maxHeightMultiplier = 0.5f; // cuánto sube la flecha según la distancia
	public float minRotationSpeed = 180f;    // velocidad mínima de rotación (grados/seg)
	public float maxRotationSpeed = 360f;    // velocidad máxima de rotación (grados/seg)

	private Vector3 startPos;
	private Vector3 targetPos = Vector3.zero;
	private float journeyLength;
	private float travelTime;
	private float startTime;

	private bool isFlying = false;
	private float rotationSpeed; // velocidad de giro aleatoria

	EnemyAttackPoints target;
	public void Shoot(EnemyAttackPoints target)
	{
		this.target = target;
		startPos = transform.position;
		targetPos = target.transform.position;
		startTime = Time.time;
		isFlying = true;
		rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);

	}

	void Update()
	{
		if (!isFlying) return;

		Vector3 targetPosSave = targetPos;


		if (target != null)
		{
			if (!target.gameObject.activeSelf)
				target = null;
			else
				targetPosSave = target.transform.position;
		}


		targetPos = targetPosSave;
		journeyLength = Vector3.Distance(startPos, targetPos);
		travelTime = journeyLength / speed; // tiempo que tardará

		float elapsed = Time.time - startTime;
		float t = Mathf.Clamp01(elapsed / travelTime); // 0 → 1

		// Altura máxima proporcional a la distancia
		float maxHeight = journeyLength * maxHeightMultiplier;
		float height = 4 * maxHeight * t * (1 - t); // parábola simple

		// Posición interpolada
		Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
		pos.y += height;
		transform.position = pos;
		transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

		// Rotación hacia la tangente de la parábola
		if (t < 1f)
		{
			// Calculamos la dirección aproximada a seguir usando derivada de la parábola
			float deltaT = 0.01f;
			float nextT = Mathf.Clamp01(t + deltaT);
			Vector3 nextPos = Vector3.Lerp(startPos, targetPos, nextT);
			float nextHeight = 4 * maxHeight * nextT * (1 - nextT);
			nextPos.y += nextHeight;

			Vector3 dir = nextPos - pos;

		}

		// Llegó al target
		if (t >= 1f)
		{
			target?.GetDamage();
			isFlying = false;
			Destroy(gameObject); // o efecto de impacto
		}
	}
}
