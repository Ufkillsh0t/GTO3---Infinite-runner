using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour
{

    public PlayerScript player;
    public PlayerSoundScript pss;
    public int deathDistance = 2;
    public int defaultDistance = 5;
    public float enemySpeedPlayer = 0.9f;
    public float enemySpeed = 2f;

    // Use this for initialization
    void Start()
    {
        if (player == null) player = gameObject.GetComponentInParent<PlayerScript>();
        if (pss == null) pss = gameObject.GetComponentInParent<PlayerSoundScript>();
        transform.localPosition = new Vector3(0, 0, -defaultDistance);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CheckPlayerDeath();
    }

    private void CheckPlayerDeath()
    {
        if (transform.localPosition.z >= -deathDistance)
        {
            pss.PlayDeathSound();
        }
    }

    private void Move()
    {
        if (player.currentSpeedMultiplier < enemySpeedPlayer)
        {
            transform.localPosition = new Vector3(0, 0,
                    transform.localPosition.z + (enemySpeedPlayer - player.currentSpeedMultiplier) * Time.deltaTime * enemySpeed);
        }
        if (player.currentSpeedMultiplier > enemySpeedPlayer)
        {
            if (transform.localPosition.z <= -defaultDistance) return;

            float distanceIncrease = (player.currentSpeedMultiplier - enemySpeedPlayer) * Time.deltaTime * enemySpeed;

            if ((transform.localPosition.z - distanceIncrease) >= -defaultDistance)
            {
                float distance = transform.localPosition.z - distanceIncrease;
                transform.localPosition = new Vector3(0, 0, distance);
            }
            else
            {
                transform.localPosition = new Vector3(0, 0, -defaultDistance);
            }
        }
    }
}
