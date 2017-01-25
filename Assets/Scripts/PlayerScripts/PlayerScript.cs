using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private enum HitDirection
    {
        None,
        Top,
        Bottom,
        Forward,
        Back,
        Left,
        Right
    }

    public bool stream_mobile_debug = true;
    public bool moveAccelerometer = false;
    public bool moveSlideDistance = true;

    public float xSpeed = 10f;
    public float zSpeed = 10f;
    public float mobileSpeedMultiplier = 0.25f;
    public float maxSwipeMultiplier = 1.2f;
    public float speedSlowdown = 0.25f;
    public float currentSpeedMultiplier = 1.0f;
    public float minSpeed = 0.5f;
    public float defaultSpeed = 1.0f;
    public float maxSpeed = 2f;

    public bool grounded = false;
    private bool death = false;

    public float jumpSpeed = 300f;
    public float gravity = 9.81f;
    public float deathPosY = -8f;

    [Range(100, 500)]
    public float feetMin = 350f;
    [Range(500, 600)]
    public float feetMax = 450f;

    private float feetTime = 0f;

    public Text scoreText;
    public int collectedCoins = 0;

    public bool magnetUsed = false;
    public float magnetPickupRange = 5f;
    private float magnetDuration = 12f;
    //public LayerMask collisionLayer;

    private Rigidbody rigidBody;
    private BoxCollider box;
    private GestureController controller;
    private IPlatform pm;

    public PlayerSoundScript pss;
    public ParticleSystem coinParticleSystem;
    public ParticleSystem magnetParticleSystem;
    public ParticleSystem landParticleSystem;

    private EnemyScript enemy;
    private static PlayerScript instance;

    public BoxCollider BoxCollider { get { return box; } set { box = value; } }
    public float MagnetDuration { get { return magnetDuration; } set { magnetDuration = value; } }
    public IPlatform GetPlatForm { get { return pm; } }
    public EnemyScript GetEnemy { get { return enemy; } }
    public static PlayerScript Instance { get { return instance; } }

    void Awake()
    {
        instance = this;
        controller = gameObject.GetComponent<GestureController>();
        rigidBody = gameObject.GetComponent<Rigidbody>();
        box = gameObject.GetComponent<BoxCollider>();
        pss = gameObject.GetComponent<PlayerSoundScript>();
        enemy = gameObject.GetComponentInChildren<EnemyScript>();
        bool toggled = (PlayerPrefs.GetInt(PrefKeys.SlideMovement.ToString()) == 1) ? true : false;
        if (toggled)
        {
            moveSlideDistance = true;
            moveAccelerometer = false;
        }
        else
        {
            moveSlideDistance = false;
            moveAccelerometer = true;
        }

        if (scoreText != null) scoreText.text = collectedCoins.ToString();
    }

    // Use this for initialization
    void Start()
    {
        Physics.gravity = new Vector3(0, -gravity, 0);
        pm = GameObject.FindGameObjectWithTag("TerrainGenerator").GetComponent<IPlatform>();
    }

    void OnCollisionEnter(Collision col)
    {
        HitDirection hitDir = ReturnHitDirection(col.transform, this.transform);
        Debug.Log(hitDir + col.gameObject.layer.ToString());
        if (hitDir == HitDirection.Top || hitDir == HitDirection.None)
        {
            pss.PlayLanding();
            grounded = true;
            zSpeed = 10f;
            if (landParticleSystem != null) landParticleSystem.Play();
        }
        else
        {
            transform.position.Set(transform.position.x, transform.position.y, transform.position.z - zSpeed * Time.deltaTime);
            zSpeed = 0;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        grounded = false;
    }

    private HitDirection ReturnHitDirection(Transform col, Transform hit)
    {
        RaycastHit rayHit;
        Vector3 direction = (col.position - hit.position).normalized;
        Ray ray = new Ray(hit.position, direction);
        Debug.DrawRay(hit.position, direction, Color.red);
        HitDirection hitDir = HitDirection.None;

        int layer = 1 << 9;
        //Debug.Log("LayerTest" + layer);
        if (Physics.Raycast(ray, out rayHit, 20, layer))
        {
            if (rayHit.collider != null)
            {
                Debug.Log(rayHit.collider.gameObject.layer);
                Vector3 normal = rayHit.normal;
                normal = rayHit.transform.TransformDirection(normal);
                if (normal == rayHit.transform.forward) hitDir = HitDirection.Forward;
                if (normal == -rayHit.transform.forward) hitDir = HitDirection.Back;
                if (normal == rayHit.transform.right) hitDir = HitDirection.Right;
                if (normal == -rayHit.transform.right) hitDir = HitDirection.Left;
                if (normal == rayHit.transform.up) hitDir = HitDirection.Top;
                if (normal == -rayHit.transform.up) hitDir = HitDirection.Bottom;
            }
        }
        return hitDir;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(Input.acceleration.x * Time.deltaTime, 0, Input.acceleration.z * Time.deltaTime);

        if (SystemInfo.deviceType == DeviceType.Desktop && !stream_mobile_debug)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            SetCurrentSpeed(vertical);

            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
                //transform.Translate(xSpeed * horizontal * Time.deltaTime, jumpSpeed * Time.deltaTime, zSpeed * vertical * Time.deltaTime);
            }
            //transform.Translate(xSpeed * horizontal * Time.deltaTime, 0, zSpeed * vertical * Time.deltaTime);
            transform.Translate(xSpeed * horizontal * Time.deltaTime, 0, 0);
            Move();
        }
        else if (SystemInfo.deviceType != DeviceType.Console || stream_mobile_debug)
        {
            MobileInput();
        }

        if (transform.position.y < deathPosY)
        {
            Death(false);
        }

        if (scoreText != null) scoreText.text = collectedCoins.ToString();

        PlayFeet();
        MagnetUsed();
    }

    private void SetCurrentSpeed(float input)
    {
        if (input < -0.5f)
        {
            if (currentSpeedMultiplier > minSpeed)
            {
                currentSpeedMultiplier -= Time.deltaTime * speedSlowdown;
                CheckSpeed();
            }
        }
        else if (input <= 0.5f)
        {
            if (currentSpeedMultiplier < defaultSpeed)
            {
                currentSpeedMultiplier += Time.deltaTime * speedSlowdown;
                if (currentSpeedMultiplier > defaultSpeed) currentSpeedMultiplier = defaultSpeed;
            }
            else
            {
                currentSpeedMultiplier -= Time.deltaTime * speedSlowdown;
                if (currentSpeedMultiplier < defaultSpeed) currentSpeedMultiplier = defaultSpeed;
            }
        }
        else if (input > 0.5f)
        {
            if (currentSpeedMultiplier < maxSpeed)
            {
                currentSpeedMultiplier += Time.deltaTime * speedSlowdown;
                CheckSpeed();
            }
        }
    }

    private void CheckSpeed()
    {
        if (currentSpeedMultiplier < minSpeed) currentSpeedMultiplier = minSpeed;
        if (currentSpeedMultiplier > maxSpeed) currentSpeedMultiplier = maxSpeed;
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
                PickUpNearbyCoin();
            }
            else
            {
                magnetUsed = false;
            }
        }
    }

    private void PickUpNearbyCoin()
    {
        if (pm != null)
        {
            pm.Pickup(magnetPickupRange);
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
            if (controller.currentState == Gestures.Touch)
            {
                Jump();
            }
            SetCurrentSpeed(-Input.acceleration.z + 0.5f);
            Move();
            transform.Translate(Input.acceleration.x * xSpeed * mobileSpeedMultiplier * Time.deltaTime, 0, 0);
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
            Move();
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
                case Gestures.TwoFingerSwipeOutwards:
                    SetCurrentSpeed(1);
                    break;
                case Gestures.TwoFingerTouch:
                    SetCurrentSpeed(-1);
                    break;
            }
        }
    }

    private void UseItem()
    {
        //Debug.Log("Used an item");
    }

    private void Jump()
    {
        if (grounded)
        {
            rigidBody.AddForce(Vector3.up * jumpSpeed);
            grounded = false;
            pss.PlayJump();
        }
    }

    private void Slide()
    {
        //Debug.Log("Slide");
    }

    private void MoveLeft()
    {
        if (moveSlideDistance)
        {
            float swipeMultiplier = controller.swipeDistanceCurrentStateX < maxSwipeMultiplier ? controller.swipeDistanceCurrentStateX : maxSwipeMultiplier;
            transform.Translate(-xSpeed * swipeMultiplier * mobileSpeedMultiplier * Time.deltaTime, 0f, 0f);
        }
        else
        {
            transform.Translate(-xSpeed * mobileSpeedMultiplier * Time.deltaTime, 0f, 0f);
        }
    }

    private void MoveRight()
    {
        if (moveSlideDistance)
        {
            float swipeMultiplier = controller.swipeDistanceCurrentStateX < maxSwipeMultiplier ? controller.swipeDistanceCurrentStateX : maxSwipeMultiplier;
            transform.Translate(xSpeed * swipeMultiplier * mobileSpeedMultiplier * Time.deltaTime, 0f, 0f);
        }
        else
        {
            transform.Translate(xSpeed * mobileSpeedMultiplier * Time.deltaTime, 0f, 0f);
        }
    }

    private void Move()
    {
        transform.Translate(0f, 0f, zSpeed * mobileSpeedMultiplier * currentSpeedMultiplier * Time.deltaTime);
    }

    private void ColliderReset()
    {
        box.size = new Vector3(1, 1, 1);
    }

    public void Death(bool instantPlay)
    {
        if (!death)
        {
            //Time.timeScale = 0;
            pss.PlayDeathSound(instantPlay);
            death = true;
        }
    }

    /// <summary>
    /// Sets the currentSpeedMultiplier between the min and max speed multipliers.
    /// </summary>
    /// <param name="speedMultiplier">The multiplier you want.</param>
    public void SetSpeed(float speedMultiplier)
    {
        currentSpeedMultiplier = speedMultiplier;
        CheckSpeed();
    }
}
