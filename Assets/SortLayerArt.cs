using UnityEngine;

public class SortLayerArt : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(SpriteRenderer render in this.transform.GetComponentsInChildren<SpriteRenderer>())
        {
            render.sortingOrder = 100000 - ((int)((render.transform.position.y+50) * 100) * 5);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
