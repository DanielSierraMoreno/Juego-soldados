using UnityEngine;

public class SortLayerArt : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			SpriteRenderer render = transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (render != null)
			    render.sortingOrder = 100000 - ((int)((render.transform.position.y + 50) * 100) * 5);
		}

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
