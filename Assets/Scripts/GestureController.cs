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
    TwoFingerSwipeOutwards
}

public class GestureController : MonoBehaviour
{
    public Gestures lastGesture = Gestures.None;
    public Gestures currentGesture = Gestures.None;
    public float minimalSwipeDistanceY = 0.10f;
    public float minimalSwipeDistanceX = 0.10f;

    public Vector2 startPosition;
    public Vector2 endPosition;
    //public Vector2 startPosition2;

    // Update is called once per frame
    void Update()
    {

        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPosition = touch.position;
                    break;
                case TouchPhase.Ended:
                    lastGesture = currentGesture;
                    currentGesture = SetTouchGesture(touch);
                    break;
            }
        }
        else
        {
            if (currentGesture != Gestures.None) lastGesture = currentGesture;
            currentGesture = Gestures.None;
        }
    }

    public Gestures SetTouchGesture(Touch touch)
    {
        endPosition = touch.position;

        float swipeValueY = touch.position.y - startPosition.y;
        float swipeDistanceY = (new Vector2(0, startPosition.y) - new Vector2(0, touch.position.y)).magnitude;

        float swipeValueX = touch.position.x - startPosition.x;
        float swipeDistanceX = (new Vector2(0, startPosition.x) - new Vector2(0, touch.position.x)).magnitude;

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
