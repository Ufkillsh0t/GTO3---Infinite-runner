using UnityEngine;
using System.Collections;

public class MagnetScript : MonoBehaviour
{

    private static PlayerScript player;
    private Vector3 startPosition;
    private float coliderScale = 2f;
    private float duration = 12f;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }

    // Use this for initialization
    void Start()
    {
        if (player == null) Debug.LogWarning("Player script could not be found!");
    }

    void OnTriggerEnter(Collider other)
    {
        transform.position = startPosition;
        if (!player.magnetUsed)
        {
            player.BoxCollider.size = new Vector3(player.BoxCollider.size.x * coliderScale, 
                player.BoxCollider.size.y, 
                player.BoxCollider.size.z * coliderScale);
            player.magnetUsed = true;
        }
        player.MagnetDuration = duration; 
        Debug.Log("I got triggered!");
    }
}
