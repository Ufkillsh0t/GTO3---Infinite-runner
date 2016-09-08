using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    public bool stream_mobile_debug = true;
    public float xSpeed = 10f;
    public float zSpeed = 10f;
    public float mobileSpeedMultiplier = 0.25f;
    public float jumpSpeed = 10f;
    public float deathPosY = -6f;

    // Use this for initialization
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(Input.acceleration.x * Time.deltaTime, 0, Input.acceleration.z * Time.deltaTime);

        if (SystemInfo.deviceType == DeviceType.Desktop && !stream_mobile_debug)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (Input.GetKey(KeyCode.Space))
            {
                transform.Translate(xSpeed * horizontal * Time.deltaTime, jumpSpeed * Time.deltaTime, zSpeed * vertical * Time.deltaTime);
            }
            else
            {
                transform.Translate(xSpeed * horizontal * Time.deltaTime, 0, zSpeed * vertical * Time.deltaTime);
            }
        }
        else if (SystemInfo.deviceType != DeviceType.Console || stream_mobile_debug)
        {
            if (Input.touchCount > 0)
            {
                transform.Translate(Input.acceleration.x * xSpeed * mobileSpeedMultiplier * Time.deltaTime,
                                    jumpSpeed * Time.deltaTime,
                                    -Input.acceleration.z * zSpeed * mobileSpeedMultiplier * Time.deltaTime);
            }
            else
            {
                transform.Translate(Input.acceleration.x * xSpeed * mobileSpeedMultiplier * Time.deltaTime, 0, -Input.acceleration.z * zSpeed * mobileSpeedMultiplier * Time.deltaTime);
            }
        }

        if (transform.position.y < deathPosY)
        {
            SceneManager.LoadScene(0);
        }
    }
}
