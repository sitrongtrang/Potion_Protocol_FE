using UnityEngine;

public class SkillRingController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<SpriteRenderer>().sprite) transform.Rotate(0f, 0f, 360f * Time.deltaTime);
    }
}
