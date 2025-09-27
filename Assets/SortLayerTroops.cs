using UnityEngine;

public class SortLayerTroops : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

	}

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < transform.childCount; i++)
		{
            SpriteRenderer render = transform.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>();
			render.sortingOrder = 100000 - ((int)((render.transform.GetChild(0).position.y + 50) * 100) * 5);
		}
    }
}
