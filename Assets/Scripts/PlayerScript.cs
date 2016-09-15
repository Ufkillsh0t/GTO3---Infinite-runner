using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    public bool stream_mobile_debug = true;
    public bool moveAccelerometer = false;
    public float xSpeed = 10f;
    public float zSpeed = 10f;
    public float mobileSpeedMultiplier = 0.25f;
    public float swipeSpeed = 1.5f;

    public float jumpSpeed = 10f;
    public float deathPosY = -6f;

    private GestureController controller;

    void Awake()
    {
        controller = gameObject.GetComponent<GestureController>();
    }

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
            MobileInput();
        }

        if (transform.position.y < deathPosY)
        {
            SceneManager.LoadScene(0);
        }
    }

    private void MobileInput()
    {
        if (moveAccelerometer)
        {
            AccelerometerControles();
        }
        else
        {
            GestureControles();
        }
    }

    private void AccelerometerControles()
    {
        if (controller != null)
        {
            if (controller.currentGesture == Gestures.Touch)
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
        else
        {
            Debug.LogError("Gesture controller not found");
        }
    }

    private void GestureControles()
    {
        if (controller != null)
        {
            if (controller.currentGesture != Gestures.None)
            {
                switch (controller.currentGesture)
                {
                    case Gestures.Touch:
                        UseItem();
                        break;
                    case Gestures.SwipeDown:
                        Slide();
                        break;
                    case Gestures.SwipeDownLeft:
                        Slide();
                        MoveLeft();
                        break;
                    case Gestures.SwipeDownRight:
                        Slide();
                        MoveRight();
                        break;
                    case Gestures.SwipeLeft:
                        MoveLeft();
                        break;
                    case Gestures.SwipeRight:
                        MoveRight();
                        break;
                    case Gestures.SwipeUp:
                        Jump();
                        break;
                    case Gestures.SwipeUpperLeft:
                        Jump();
                        MoveLeft();
                        break;
                    case Gestures.SwipeUpperRight:
                        Jump();
                        MoveRight();
                        break;
                    case Gestures.TwoFingerSwipeOutwards:
                        Debug.Log("Outwards swipe");
                        break;
                    case Gestures.TwoFingerTouch:
                        Debug.Log("Two finger touch");
                        break;
                }
            }
            Move();
        }
        else
        {
            Debug.LogError("Gesture controller not found");
        }
    }

    private void UseItem()
    {
        Debug.Log("Used an item");
    }

    private void Jump()
    {
        transform.Translate(0, jumpSpeed, 0);
        Debug.Log("Jump");
    }

    private void Slide()
    {
        Debug.Log("Slide");
    }

    private void MoveLeft()
    {
        transform.Translate(-swipeSpeed, 0f, 0f);
        Debug.Log("Moved to the left");
    }

    private void MoveRight()
    {
        transform.Translate(swipeSpeed, 0f, 0f);
        Debug.Log("Moved to the right");
    }

    private void Move()
    {
        transform.Translate(0f, 0f, xSpeed * Time.deltaTime);
    }
}
