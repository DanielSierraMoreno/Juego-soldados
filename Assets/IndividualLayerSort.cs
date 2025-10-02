using UnityEngine;

public class IndividualLayerSort : MonoBehaviour
{
	SpriteRenderer render;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        render = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
		render.sortingOrder = 100000 - ((int)((render.transform.parent.position.y + 40) * 100) * 5);
	}
}
