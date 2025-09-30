using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems; // Necesario para detectar clicks en UI

public class PlayerController : MonoBehaviour
{
	public static PlayerController Instance { get; private set; }

	public FixedJoystick joystick, attackJoystick;

	public Troop currentPlayer;

	public CinemachineCamera camera;

	public GameObject cameraFollow;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		Application.targetFrameRate = 60;

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
		attackJoystick?.gameObject.SetActive(currentPlayer != null && currentPlayer?.troopType != Troop.Type.PAWN);
		joystick?.gameObject.SetActive(currentPlayer != null);

		if (attackJoystick?.Direction.magnitude > 0.2f)
		{
			if (currentPlayer != null)
			{
				currentPlayer.Attack(attackJoystick.Direction);
			}
		}

		if (Input.touchCount == 2)
		{
			Touch touch0 = Input.GetTouch(0);
			Touch touch1 = Input.GetTouch(1);

			if (EventSystem.current != null)
			{
				// Si cualquiera de los dos dedos está tocando UI → no hacemos zoom
				if (EventSystem.current.IsPointerOverGameObject(touch0.fingerId) ||
					EventSystem.current.IsPointerOverGameObject(touch1.fingerId))
				{
					return;
				}
			}

			// Posiciones en este frame y en el anterior
			Vector2 touch0Prev = touch0.position - touch0.deltaPosition;
			Vector2 touch1Prev = touch1.position - touch1.deltaPosition;

			float prevMagnitude = (touch0Prev - touch1Prev).magnitude;
			float currentMagnitude = (touch0.position - touch1.position).magnitude;

			float difference = currentMagnitude - prevMagnitude;

			// Zoom → modificar OrthographicSize
			float newZoom = camera.Lens.OrthographicSize - difference * 0.75f * Time.deltaTime;
			newZoom = Mathf.Clamp(newZoom, 4, 8);

			camera.Lens.OrthographicSize = newZoom;

			return; // 👈 IMPORTANTE: salimos del Update, no procesamos nada más

		}

		if (Input.GetMouseButtonDown(0)) // click izquierdo
		{
			if (EventSystem.current != null)
			{
				#if UNITY_EDITOR || UNITY_STANDALONE
								// En PC / Editor se usa el mouse
								if (EventSystem.current.IsPointerOverGameObject())
									return;
				#elif UNITY_ANDROID || UNITY_IOS
					// En móvil se usa el primer toque
					if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
						return;
				#endif
			}

			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			// Lanzamos un raycast en el punto del click
			Collider2D[] hits = Physics2D.OverlapPointAll(mousePos);

			if (hits.Length > 0)
			{
				Collider2D closest = null;
				float minDist = float.MaxValue;

				foreach (Collider2D c in hits)
				{
					if (c.CompareTag("Troop"))
					{
						float dist = Vector2.Distance(mousePos, c.transform.position);
						if (dist < minDist)
						{
							minDist = dist;
							closest = c;
						}
					}
				}

				if (closest != null)
				{
					// Des-seleccionar si había otro
					if (currentPlayer != null)
					{
						currentPlayer.DeSelect();
					}

					currentPlayer = closest.GetComponentInParent<Troop>();
					currentPlayer.Select();

				}
				else
				{
					// No había "Troop" bajo el dedo
					if (currentPlayer != null)
					{
						currentPlayer.DeSelect();
						currentPlayer = null;
					}
				}
			}
			else
			{
				// Nada clicado
				if (currentPlayer != null)
				{
					currentPlayer.DeSelect();
					currentPlayer = null;
				}
			}
		}

		if (currentPlayer == null)
		{
#if UNITY_EDITOR || UNITY_STANDALONE
			// PC / Editor → arrastre con mouse
			if (Input.GetMouseButton(0))
			{
				float mx = Input.GetAxis("Mouse X");
				float my = Input.GetAxis("Mouse Y");

				if (Mathf.Abs(mx) > 0.05f || Mathf.Abs(my) > 0.05f)
				{
					Vector3 dir = new Vector3(-mx, -my, 0);
					camera.Follow = cameraFollow.transform;

					float t = Mathf.InverseLerp(4, 8, camera.Lens.OrthographicSize);
					// t = 0 cuando zoom = zoomMin, t = 1 cuando zoom = zoomMax

					float moveSpeed = Mathf.Lerp(5, 10, t);

					Vector3 newPos = cameraFollow.transform.position + dir * moveSpeed * Time.deltaTime;

					newPos.x = Mathf.Clamp(newPos.x, -10, 40);
					newPos.y = Mathf.Clamp(newPos.y, -6, 1);

					cameraFollow.transform.position = newPos;
				}
			}
#elif UNITY_ANDROID || UNITY_IOS
    // 📱 Móvil → arrastre con el primer dedo
    if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
    {
        Vector2 delta = Input.GetTouch(0).deltaPosition;

        if (delta.sqrMagnitude > 1f) // filtro mínimo
        {
            Vector3 dir = new Vector3(-delta.x, -delta.y, 0);
            camera.Follow = cameraFollow.transform;

			float t = Mathf.InverseLerp(4, 8, camera.Lens.OrthographicSize);
			// t = 0 cuando zoom = zoomMin, t = 1 cuando zoom = zoomMax

			float moveSpeed = Mathf.Lerp(1, 2, t);

			Vector3 newPos = cameraFollow.transform.position + dir * moveSpeed * Time.deltaTime;

            newPos.x = Mathf.Clamp(newPos.x, -10, 40);
            newPos.y = Mathf.Clamp(newPos.y, -6, 1);

            cameraFollow.transform.position = newPos;
        }
    }
#endif
		}


	}
	private Vector3 velocity = Vector3.zero;

	void FixedUpdate()
	{
		if(joystick != null)
		{

			currentPlayer?.Move(joystick.Direction.normalized);


			if (camera.Follow != cameraFollow.transform && camera.Follow != null)
				cameraFollow.transform.position = camera.Follow.transform.position;
			

			if (currentPlayer != null)
			{
				camera.Follow = currentPlayer.transform;
			}



		}


	}
}
