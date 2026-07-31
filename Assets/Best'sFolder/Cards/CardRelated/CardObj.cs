using UnityEngine;

public class CardObj : MonoBehaviour
{
    [SerializeField] Card card;
    public Vector3 Belong;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, Belong, Time.deltaTime * 5); 
    }
}
