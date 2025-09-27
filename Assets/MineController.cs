using UnityEngine;
using UnityEngine.UI;

public class MineController : MonoBehaviour
{
    public Mine[] mines;
    public Button button;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public static MineController Instance { get; private set; }

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
        bool active = false;
        foreach (Mine mine in mines)
        {
            if(mine.isPlayer)
            {
                if(mine.player.state == Pawn.State.IDLE || mine.player.state == Pawn.State.MOVING ) 
                { 
                    if(!mine.player.haveObject)
                		active = true;

                }

			}
        }
        if (active)
        {
			button.gameObject.SetActive(true);

		}
        else
        {
			button.gameObject.SetActive(false);

		}
	}


    public void Interact()
    {
		foreach (Mine mine in mines)
		{
			if (mine.isPlayer)
			{
				mine.StartMining(); 
			}
		}
	}

	public Vector3 GetMineUnocupped(Vector3 pos)
	{
		Vector3 target = Vector3.zero;
		float distance = 1000000;
		foreach (Mine mine in mines)
		{
			if (!mine.mining)
			{ 
				if (Vector3.Distance(pos, mine.transform.position) < distance)
				{
					distance = Vector3.Distance(pos, mine.transform.position);
					target = mine.transform.position;
				}
			}
		}
		return target;
	}
}
