using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float speed = 5f;             
	public Rigidbody2D rb;                
	public Animator anim;
	private float moveInputX;
	private float moveInputY;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {

    }

	// Update is called once per frame
	void Update()
	{
		// Capturamos la entrada horizontal (A/D o Flechas)
		moveInputX = Input.GetAxisRaw("Horizontal");
		moveInputY = Input.GetAxisRaw("Vertical");

		// Activar animaci�n de caminar
		anim.SetBool("isRunning", moveInputX != 0 || moveInputY != 0);

		// Voltear sprite seg�n direcci�n
		if (moveInputX > 0)
			transform.localScale = new Vector3(1, 1, 1);
		else if (moveInputX < 0)
			transform.localScale = new Vector3(-1, 1, 1);

	}

	void FixedUpdate()
	{
		// Movimiento del Rigidbody2D
		rb.linearVelocity = new Vector2(moveInputX, moveInputY).normalized * speed;
	}
}
