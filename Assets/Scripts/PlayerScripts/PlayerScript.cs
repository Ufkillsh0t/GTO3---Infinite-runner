using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    public bool stream_mobile_debug = true;
    public bool moveAccelerometer = false;
    public bool moveSlideDistance = true;

    public float xSpeed = 10f;
    public float zSpeed = 10f;
    public float mobileSpeedMultiplier = 0.25f;
    public float maxSwipeMultipler = 1.2f;

    public bool grounded = false;

    public float jumpSpeed = 300f;
    public float gravity = 9.81f;
    public float deathPosY = -6f;

    [Range(100, 500)]
    public float feetMin = 350f;
    [Range(500, 600)]
    public float feetMax = 450f;

    private float feetTime = 0f;

    public int collectedCoins = 0;

    public bool magnetUsed = false;
    public int magnetPickupRange = 5;
    private float magnetDuration = 12f;


    private Rigidbody rigidBody;
    private BoxCollider box;
    private GestureController controller;
    private PlatformManager pm;

    public PlayerSoundScript pss;

    public BoxCollider BoxCollider { get { return box; } set { box = value; } }
    public float MagnetDuration { get { return magnetDuration; } set { magnetDuration = value; } }

    void Awake()
    {
        controller = gameObject.GetComponent<GestureController>();
        rigidBody = gameObject.GetComponent<Rigidbody>();
        box = gameObject.GetComponent<BoxCollider>();
        pss = gameObject.GetComponent<PlayerSoundScript>();
        if (moveAccelerometer == true && moveSlideDistance == true) moveSlideDistance = false;
    }

    // Use this for initialization
    void Start()
    {
        Physics.gravity = new Vector3(0, -gravity, 0);
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    void OnCollisionEnter(Collision col)
    {
        pss.PlayLanding();
        grounded = true;
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
                Jump();
                //transform.Translate(xSpeed * horizontal * Time.deltaTime, jumpSpeed * Time.deltaTime, zSpeed * vertical * Time.deltaTime);
            }
            transform.Translate(xSpeed * horizontal * Time.deltaTime, 0, zSpeed * vertical * Time.deltaTime);
        }
        else if (SystemInfo.deviceType != DeviceType.Console || stream_mobile_debug)
        {
            MobileInput();
        }

        if (transform.position.y < deathPosY)
        {
            pss.PlayDeathSound();
        }

        PlayFeet();
        MagnetUsed();
    }

    private void PlayFeet()
    {
        if (!pss.playerMovement.isPlaying && feetTime < 0)
        {

            feetTime = Random.Range(feetMin, feetMax);
            pss.PlayFootSteps();
        }
        else
        {
            feetTime -= Time.deltaTime;
        }
    }

    private void MagnetUsed()
    {
        if (magnetUsed)
        {
            if (magnetDuration > 0f)
            {
                magnetDuration -= Time.deltaTime;
            }
            else
            {
                magnetUsed = false;
                ColliderReset();
            }
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
                Jump();
                /*transform.Translate(Input.acceleration.x * xSpeed * mobileSpeedMultiplier * Time.deltaTime,
                                    jumpSpeed * Time.deltaTime,
                                    -Input.acceleration.z * zSpeed * mobileSpeedMultiplier * Time.deltaTime);*/
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
            GestureStates();
            GestureMoves();
            if (!moveAccelerometer) Move();
        }
        else
        {
            Debug.LogError("Gesture controller not found");
        }
    }

    private void GestureMoves()
    {
        if (controller.currentGesture != Gestures.None)
        {
            switch (controller.currentGesture)
            {
                case Gestures.Touch:
                    UseItem();
                    break;
                case Gestures.TwoFingerSwipeOutwards:
                    Debug.Log("Outwards swipe");
                    break;
                case Gestures.TwoFingerTouch:
                    Debug.Log("Two finger touch");
                    break;
            }
        }
    }

    private void GestureStates()
    {
        if (controller.currentState != Gestures.None)
        {
            switch (controller.currentState)
            {
                case Gestures.SwipeDownLeft:
                    MoveLeft();
                    Slide();
                    break;
                case Gestures.SwipeDownRight:
                    MoveRight();
                    Slide();
                    break;
                case Gestures.SwipeLeft:
                    MoveLeft();
                    break;
                case Gestures.SwipeRight:
                    MoveRight();
                    break;
                case Gestures.SwipeUpperLeft:
                    MoveLeft();
                    Jump();
                    break;
                case Gestures.SwipeUpperRight:
                    MoveRight();
                    Jump();
                    break;
                case Gestures.SwipeUp:
                    Jump();
                    break;
                case Gestures.SwipeDown:
                    Slide();
                    break;
            }
        }
    }

    private void UseItem()
    {
        Debug.Log("Used an item");
    }

    private void Jump()
    {
        if (grounded)
        {
            rigidBody.AddForce(Vector3.up * jumpSpeed);
            grounded = false;
            pss.PlayJump();
            Debug.Log("Jump");
        }
    }

    private void Slide()
    {
        Debug.Log("Slide");
    }

    private void MoveLeft()
    {
        if (moveSlideDistance)
        {
            float swipeMultiplier = controller.swipeDistanceCurrentStateX < maxSwipeMultipler ? controller.swipeDistanceCurrentStateX : maxSwipeMultipler;
            transform.Translate(-xSpeed * swipeMultiplier * mobileSpeedMultiplier * Time.deltaTime, 0f, 0f);
            Debug.Log("Multiplier " + swipeMultiplier);
        }
        else
        {
            transform.Translate(-xSpeed * mobileSpeedMultiplier * Time.deltaTime, 0f, 0f);
        }
        Debug.Log("Moved to the left");
    }

    private void MoveRight()
    {
        if (moveSlideDistance)
        {
            float swipeMultiplier = controller.swipeDistanceCurrentStateX < maxSwipeMultipler ? controller.swipeDistanceCurrentStateX : maxSwipeMultipler;
            Debug.Log("Multiplier " + swipeMultiplier);
            transform.Translate(xSpeed * controller.swipeDistanceCurrentStateX * mobileSpeedMultiplier * Time.deltaTime, 0f, 0f);
        }
        else
        {
            transform.Translate(xSpeed * mobileSpeedMultiplier * Time.deltaTime, 0f, 0f);
        }
        Debug.Log("Moved to the right");
    }

    private void Move()
    {
        transform.Translate(0f, 0f, zSpeed * mobileSpeedMultiplier * Time.deltaTime);
    }

    private void ColliderReset()
    {
        box.size = new Vector3(1, 1, 1);
    }
}
