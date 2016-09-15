using UnityEngine;
using System.Collections;

public enum Gestures
{
    None,
    Touch,
    SwipeUp,
    SwipeUpperLeft,
    SwipeUpperRight,
    SwipeDown,
    SwipeDownLeft,
    SwipeDownRight,
    SwipeLeft,
    SwipeRight,
    TwoFingerSwipeInwards,
    TwoFingerSwipeOutwards,
    TwoFingerTouch
}

public enum Finger
{
    One,
    Two
}

public class GestureController : MonoBehaviour
{

    public Gestures gestureFinger1 = Gestures.None;
    public Gestures gestureFinger2 = Gestures.None;
    //LastGesture that has been used.
    public Gestures lastGesture = Gestures.None;
    //CurrentGesture after completed.
    public Gestures currentGesture = Gestures.None;
    //CurrentGesture in progress. (That hasn't been done yet)
    public Gestures currentState = Gestures.None;

    public float swipeDistanceCurrentStateDivider = 200f;
    public float swipeDistanceCurrentStateX = 0f;
    public float swipeDistanceCurrentStateY = 0f;

    public float minimalSwipeDistanceY = 0.10f;
    public float minimalSwipeDistanceX = 0.10f;
    public bool twoFingerTouch = true;

    public Vector2 startPosition;
    public Vector2 startPosition2;
    public Vector2 endPosition;
    public Vector2 endPosition2;

    private bool multitouch = false;
    //public Vector2 startPosition2;

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            GetGesture();
        }
        else
        {
            if (currentGesture != Gestures.None) lastGesture = currentGesture;
            currentGesture = Gestures.None;
            currentState = Gestures.None;
        }
    }

    public void GetGesture()
    {
        if (Input.touchCount == 1 && !multitouch)
        {
            GetOneFingerGesture();
        }
        else if (Input.touchCount == 2 || multitouch && twoFingerTouch)
        {
            GetTwoFingerGesture();
        }
    }

    public void GetOneFingerGesture()
    {
        Touch touch = Input.touches[0];
        switch (touch.phase)
        {
            case TouchPhase.Began:
                startPosition = touch.position;
                break;
            case TouchPhase.Moved: //This case needs to be tested
                currentState = SetTouchGesture(touch, Finger.One);
                break;
            case TouchPhase.Ended:
                lastGesture = currentGesture;
                currentGesture = SetTouchGesture(touch, Finger.One);
                gestureFinger1 = currentGesture;
                break;
        }
    }

    public void GetTwoFingerGesture()
    {
        multitouch = true;
        Touch touch1 = Input.touches[0];
        Touch touch2;
        if (Input.touches.Length > 1)
        {
            touch2 = Input.touches[1];
            switch (touch1.phase)
            {
                case TouchPhase.Began:
                    startPosition = touch1.position;
                    break;
                case TouchPhase.Ended:
                    gestureFinger1 = SetTouchGesture(touch1, Finger.One);
                    break;
            }
            switch (touch2.phase)
            {
                case TouchPhase.Began:
                    startPosition2 = touch1.position;
                    break;
                case TouchPhase.Ended:
                    gestureFinger2 = SetTouchGesture(touch2, Finger.Two);
                    break;
            }
            CheckMultiFingerGesture();
        }
        else if (gestureFinger1 == Gestures.None && gestureFinger2 != Gestures.None)
        {
            gestureFinger1 = SetTouchGesture(touch1, Finger.One);
            CheckMultiFingerGesture();
        }
        else if (gestureFinger1 != Gestures.None && gestureFinger2 == Gestures.None)
        {
            gestureFinger2 = SetTouchGesture(touch1, Finger.Two);
            CheckMultiFingerGesture();
        }
        else
        {
            Debug.Log("Multi touch triggered without multiple inputs");
            //indien nog een beetje glitchy plaats ResetTwoFingerGesture
        }
    }

    public void CheckMultiFingerGesture()
    {
        if (gestureFinger1 != Gestures.None && gestureFinger2 != Gestures.None)
        {
            if (gestureFinger1 == Gestures.Touch || gestureFinger2 == Gestures.Touch)
            {
                ResetTwoFingerGestures();
                currentGesture = Gestures.TwoFingerTouch;
            }
            else if (((gestureFinger1 == Gestures.SwipeLeft || gestureFinger1 == Gestures.SwipeDownLeft
               || gestureFinger1 == Gestures.SwipeUpperLeft || gestureFinger1 == Gestures.SwipeUp) &&
               (gestureFinger2 == Gestures.SwipeRight || gestureFinger2 == Gestures.SwipeDownRight ||
               gestureFinger2 == Gestures.SwipeUpperRight || gestureFinger2 == Gestures.SwipeDown)) ||
               (gestureFinger1 == Gestures.SwipeRight || gestureFinger1 == Gestures.SwipeDownRight ||
               gestureFinger1 == Gestures.SwipeUpperRight || gestureFinger1 == Gestures.SwipeDown) &&
               (gestureFinger2 == Gestures.SwipeLeft || gestureFinger2 == Gestures.SwipeDownLeft
               || gestureFinger2 == Gestures.SwipeUpperLeft || gestureFinger2 == Gestures.SwipeUp))
            {
                ResetTwoFingerGestures();
                currentGesture = Gestures.TwoFingerSwipeOutwards;
            }
            else if (gestureFinger2 == Gestures.SwipeLeft && gestureFinger1 == Gestures.SwipeRight)
            {
                ResetTwoFingerGestures();
                currentGesture = Gestures.TwoFingerSwipeInwards;
            }
            else
            {
                ResetTwoFingerGestures();
            }
        }
    }

    public void ResetTwoFingerGestures()
    {
        gestureFinger1 = Gestures.None;
        gestureFinger2 = Gestures.None;
        multitouch = false;
    }

    public Gestures SetTouchGesture(Touch touch, Finger curFinger)
    {
        float swipeValueY;
        float swipeDistanceY;

        float swipeValueX;
        float swipeDistanceX;

        if (curFinger == Finger.One)
        {
            endPosition = touch.position;
            swipeValueY = touch.position.y - startPosition.y;
            swipeDistanceY = (new Vector2(0, startPosition.y) - new Vector2(0, touch.position.y)).magnitude;

            swipeValueX = touch.position.x - startPosition.x;
            swipeDistanceX = (new Vector2(0, startPosition.x) - new Vector2(0, touch.position.x)).magnitude;
        }
        else
        {
            endPosition2 = touch.position;
            swipeValueY = touch.position.y - startPosition2.y;
            swipeDistanceY = (new Vector2(0, startPosition2.y) - new Vector2(0, touch.position.y)).magnitude;

            swipeValueX = touch.position.x - startPosition2.x;
            swipeDistanceX = (new Vector2(0, startPosition2.x) - new Vector2(0, touch.position.x)).magnitude;
        }

        swipeDistanceCurrentStateX = swipeDistanceX / swipeDistanceCurrentStateDivider;
        swipeDistanceCurrentStateY = swipeDistanceY / swipeDistanceCurrentStateDivider;

        Debug.Log("SwipeValueY:" + swipeValueY + " SwipeDistY:" + swipeDistanceY + " SwipevalueX:" + swipeValueX + " SwipeDistX:" + swipeDistanceX);

        if (swipeDistanceX > minimalSwipeDistanceX)
        {
            if (swipeDistanceY > minimalSwipeDistanceY)
            {
                return GetDiagonalSwipe(swipeValueX, swipeValueY);
            }
            else
            {
                return GetHorizontalSwipe(swipeValueX);
            }
        }
        else if (swipeDistanceY > minimalSwipeDistanceY)
        {
            return GetVerticalSwipe(swipeValueY);
        }
        else
        {
            return Gestures.Touch;
        }
    }

    private Gestures GetDiagonalSwipe(float swipeValueX, float swipeValueY)
    {
        if (swipeValueX > 0 && swipeValueY > 0)
        {
            return Gestures.SwipeUpperRight;
        }
        else if (swipeValueX > 0 && swipeValueY < 0)
        {
            return Gestures.SwipeDownRight;
        }
        else if (swipeValueX < 0 && swipeValueY < 0)
        {
            return Gestures.SwipeDownLeft;
        }
        else
        {
            return Gestures.SwipeUpperLeft;
        }
    }

    private Gestures GetHorizontalSwipe(float swipeValueX)
    {
        if (swipeValueX > 0)
        {
            return Gestures.SwipeRight;
        }
        else
        {
            return Gestures.SwipeLeft;
        }
    }

    private Gestures GetVerticalSwipe(float swipeValueY)
    {
        if (swipeValueY > 0)
        {
            return Gestures.SwipeUp;
        }
        else
        {
            return Gestures.SwipeDown;
        }
    }
}
//int touches = Input.touchCount > 2 ? 2 : Input.touchCount;
//Gestures
//for(int i = 0; i < touches)
//{
//    switch
//}
