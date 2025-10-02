using UnityEngine;

public class RandomizeAnimationStart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.GetComponent<Animator>().speed = Random.Range(0.9f, 1.1f);

	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
