using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Castle : MonoBehaviour
{

	public int money = 0;

	public TMP_Text text;
	public static Castle Instance { get; private set; }

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
	}

	// Update is called once per frame
	void Update()
    {
		text.text = money.ToString();

	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.GetComponent<Pawn>() != null)
		{
			Pawn pawn = collision.GetComponent<Pawn>();
			if (pawn.haveObject)
			{
				if (pawn == PlayerController.Instance.currentPlayer)
				{
					pawn.haveObject = false;
					money += 5;
					pawn.transform.GetChild(0).GetChild(1).GetComponent<Animator>().CrossFade("DeSpawn", 0);

				}
				else
				{
					pawn.haveObject = false;
					money += 5;
					pawn.transform.GetChild(0).GetChild(1).GetComponent<Animator>().CrossFade("DeSpawn", 0);
					pawn.agent.destination = MineController.Instance.GetMineUnocupped(pawn.transform.position);
					pawn.state = Pawn.State.MOVING;
					pawn.targetDestination = pawn.agent.destination;
					pawn.anim.CrossFade(pawn.haveObject ? "Run_0" : "Run", 0);
				}
			}



		}
	}
}
