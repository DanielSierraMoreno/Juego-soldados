using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Castle : MonoBehaviour
{

	public int money = 0;

	public TMP_Text text;
	public static Castle Instance { get; private set; }

	public Transform pos;

	public GameObject pawnPrefab;
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
					pawn.agent.isStopped = true;

					pawn.haveObject = false;
					money += pawn.money;
					pawn.transform.GetChild(0).GetChild(0).GetComponent<Animator>().CrossFade("DeSpawn", 0);

				}
				else
				{
					pawn.agent.isStopped = false;

					pawn.haveObject = false;
					money += pawn.money;
					pawn.transform.GetChild(0).GetChild(0).GetComponent<Animator>().CrossFade("DeSpawn", 0);
					pawn.agent.destination = MineController.Instance.GetMineUnocupped(pawn.transform.position);
					pawn.state = Pawn.State.MOVING;
					pawn.targetDestination = pawn.agent.destination;
					pawn.anim.CrossFade(pawn.haveObject ? "Run_0" : "Run", 0);
				}
			}



		}
	}

	public void BuyPawn()
	{
		int price = 0;

		foreach (Pawn pawn in FindObjectsOfType<Pawn>(true))
		{
			if (pawn.gameObject.activeSelf)
			{
				price = 150;
			}
		}

		if (money >= price)
		{
			foreach (Pawn pawn in FindObjectsOfType<Pawn>(true))
			{
				if (!pawn.gameObject.activeSelf)
				{
					pawn.transform.localPosition = Vector3.zero;
					pawn.gameObject.SetActive(true);
					pawn.Reset();

					money -= 150;

					if (money < 0)
						money = 0;
					return;
				}
			}

		}
	}

	public void BuyWarrior()
	{
		int price = 200;



		if (money >= price)
		{
			foreach (Warrior warrior in FindObjectsOfType<Warrior>(true))
			{
				if (!warrior.gameObject.activeSelf)
				{
					warrior.transform.localPosition = Vector3.zero;
					warrior.gameObject.SetActive(true);
					warrior.Reset();

					money -= 200;

					if (money < 0)
						money = 0;
					return;
				}
			}

		}
	}
}
