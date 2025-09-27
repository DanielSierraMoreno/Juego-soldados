using UnityEngine;

public class Troop : MonoBehaviour
{
    public enum Type { PAWN, ARCHER, LANCER, SOLDIER};

    public float moveSpeed = 5;

    public Animator anim;

    public Type troopType;

    public bool selected = false;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	protected virtual void Start()
	{

    }

	// Update is called once per frame
	protected virtual void Update()
    {
        


    }

	public virtual void Move(Vector2 dir)
	{
        if(dir.magnitude > 0)
        {
            anim.SetBool("Move", true);
        }
        else
        {
			anim.SetBool("Move", false);

		}

        if(dir.x > 0)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }
        else if(dir.x < 0)
        {
			this.transform.localScale = new Vector3(-1, 1, 1);
		}

		this.GetComponent<Rigidbody2D>().MovePosition(this.GetComponent<Rigidbody2D>().position + new Vector2(dir.x * moveSpeed * Time.deltaTime, dir.y * moveSpeed * Time.deltaTime));
	}
}
