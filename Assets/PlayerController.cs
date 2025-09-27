using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public static PlayerController Instance { get; private set; }

	public FixedJoystick joystick;

	public Troop currentPlayer;


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
		
	}

	void FixedUpdate()
	{
		if(joystick != null)
		{
			currentPlayer?.Move(joystick.Direction.normalized);
		}
	}
}
