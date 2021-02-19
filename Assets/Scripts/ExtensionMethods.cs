using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    #region Input


    /// <summary>PC/Mobile compatible, returns true on the frame the hitbox is pressed.</summary>
    /// <param name="collider">The hitbox to check for presses on.</param>
    public static bool TouchedHitbox(BoxCollider collider)
    {
        Vector3 touchPos = Vector3.zero;
        bool touched = false;

        DetectTouches(out touched, out touchPos);

        if (touched)
        {

            Collider[] colliders = Physics.OverlapSphere(touchPos, .1f);
            if (colliders.Length > 0)
            {
                foreach (var _coll in colliders)
                {
                    if (_coll == collider)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    /// <summary>PC/Mobile compatible, returns true on the frame the hitbox is pressed.</summary>
    /// <param name="collider">The hitbox to check for presses on.</param>
    /// <param name="touchPos">The position of the touch.</param>
    public static bool TouchedHitbox(BoxCollider collider, Vector3 touchPos)
    {
        Collider[] colliders = Physics.OverlapSphere(touchPos, .1f);
        if (colliders.Length > 0)
        {
            foreach (var _coll in colliders)
            {
                if (_coll == collider)
                {
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>PC/Mobile compatible, returns true on the frame the hitbox is pressed.</summary>
    /// <param name="collider">The hitbox to check for presses on.</param>
    /// <param name="touchPos">The position of the touch.</param>
    /// <param name="layerMask">The layers the touch will be detected for on.</param>
    public static bool TouchedHitbox(BoxCollider collider, Vector3 touchPos, LayerMask layerMask)
    {
        Collider[] colliders = Physics.OverlapSphere(touchPos, .1f,layerMask);
        if (colliders.Length > 0)
        {
            foreach (var _coll in colliders)
            {
                if (_coll == collider)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>Checks whether touching a given hitbox.</summary>
    /// <param name="collider">The collider to detect touches upon.</param>
    public static bool TouchingHitbox(BoxCollider collider)
    {
        Vector3 touchPos = Vector3.zero;
        bool touchHeld = false;
        bool touchedBegan = false;

        DetectTouchesHeld(out touchHeld, out touchedBegan, out touchPos);

        if (touchHeld)
        {

            Collider[] colliders = Physics.OverlapSphere(touchPos, .1f);
            if (colliders.Length > 0)
            {
                foreach (var _coll in colliders)
                {
                    if (_coll == collider)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>PC/Mobile compatible, returns whether touched on a given frame and its touch position.</summary>
    /// <param name="touched">Returns whether pressed on the given frame.</param>
    /// <param name="touchP">Returns position of touch.</param>
    public static void DetectTouches(out bool touched, out Vector3 touchP)
    {
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            bool pressedFrame = false;
            Vector3 touchPos = Vector3.zero;
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                    pressedFrame = true;
                }
            }
            else
            {
                touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pressedFrame = true;
            }

            if (pressedFrame)
            {
                touchPos.z = 0;
                touched = true;
                touchP = touchPos;
            }
            else
            {
                touched = false;
                touchP = Vector3.zero;
            }
        }
        else
        {
            touched = false;
            touchP = Vector3.zero;
        }
    }
    /// <summary>PC/Mobile compatible, returns when touched/held down and its touch position.</summary>
    /// <param name="touchHeld">Returns whether press is being held.</param>
    /// <param name="touchBegan">Returns whether pressed on the given frame.</param>
    /// <param name="touchP">Returns position of touch.</param>
    public static void DetectTouchesHeld(out bool touchHeld, out bool touchBegan, out Vector3 touchP)
    {
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            touchHeld = true;
        }
        else
        {
            touchHeld = false;
        }


        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            bool pressedFrame = false;
            Vector3 touchPos = Vector3.zero;
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {

                    pressedFrame = true;
                }

                touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            }
            else
            {
                touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (Input.GetMouseButtonDown(0))
                {
                    pressedFrame = true;
                }
            }

            if (pressedFrame)
            {
                touchPos.z = 0;
                touchBegan = true;
                touchP = touchPos;
            }
            else if (touchHeld)
            {
                touchBegan = false;
                touchPos.z = 0;
                touchP = touchPos;
            }
            else
            {
                touchBegan = false;
                touchP = Vector3.zero;
            }
        }
        else
        {
            touchBegan = false;
            touchP = Vector3.zero;
        }
    }
    /// <summary>Manages slider knob with snapped, target positions.</summary>
    /// <param name="indexReturn">Outputs the index at which the slider knob snaps. Use same variable as index</param>
    /// <param name="index">Input of current index, use same variable as indexReturn.</param>
    /// <param name="inMoveReturn">Outputs whether the player is currently sliding. Use same variable as inMove.</param>
    /// <param name="inMove">Input of current moving status, use same variable as inMoveReturn.</param>
    /// <param name="targetPositions">List of all snappable positions.</param>
    /// <param name="clampValues">The left and right clamp values for the knob, when unsnapped.</param>
    /// <param name="hitbox">Hitbox of the slider knob.</param>
    /// <param name="totalHitbox">Hitbox of the total area that will detect sliding.</param>
    public static void SliderPositionLock(this Transform _transform, out int indexReturn, int index, out bool inMoveReturn, bool inMove, Vector2[] targetPositions, Vector2 clampValues, BoxCollider hitbox, BoxCollider totalHitbox)
    {

        bool holdingDown = false;
        bool pressedFrame = false;
        Vector3 touchPos = Vector2.zero;
        

        DetectTouchesHeld(out holdingDown, out pressedFrame, out touchPos);


        if (pressedFrame && TouchedHitbox(totalHitbox, touchPos) && !TouchedHitbox(hitbox, touchPos))
        {
            int bestIndex = 0;
            float bestDistance = 999;
            for (int i = 0; i < targetPositions.Length; i++)
            {
                touchPos.y = _transform.position.y;
                float distance = Vector2.Distance(touchPos, targetPositions[i]);
                if (distance < bestDistance)
                {
                    bestIndex = i;
                    bestDistance = distance;
                }
            }

            indexReturn = bestIndex;
            inMove = true;
            inMoveReturn = inMove;
        }
        else if (holdingDown && TouchedHitbox(totalHitbox, touchPos))
        {
            Vector2 newPos = _transform.position;

            touchPos.z = 0;
            newPos.x = Mathf.Clamp(touchPos.x, clampValues.x, clampValues.y);
            _transform.position = newPos;

            int bestIndex = 0;
            float bestDistance = 999;
            for (int i = 0; i < targetPositions.Length; i++)
            {
                float distance = Vector2.Distance(touchPos, targetPositions[i]);
                if (distance < bestDistance)
                {
                    bestIndex = i;
                    bestDistance = distance;
                }
            }

            indexReturn = bestIndex;
            inMove = true;
            inMoveReturn = inMove;
        }
        else
        {
            int bestIndex = 0;
            float bestDistance = 999;
            for (int i = 0; i < targetPositions.Length; i++)
            {
                float distance = Vector2.Distance(_transform.position, targetPositions[i]);
                if (distance < bestDistance)
                {
                    bestIndex = i;
                    bestDistance = distance;
                }
            }

            int indexStore = bestIndex;

            if (inMove)
            {
                if (bestIndex == index)
                {
                    inMove = false;
                    inMoveReturn = false;
                    //AudioManager.main.PlaySelectSound(AudioManager.SelectionSound.HighSharp, .8f, .1f, 1, 0);
                }
                indexReturn = index;
            }
            else
            {
                index = indexStore;
                indexReturn = indexStore;
            }


            Vector2 targetPosition = targetPositions[index];
            targetPosition.x = Mathf.Clamp(targetPosition.x, clampValues.x, clampValues.y);
            _transform.position = Vector2.Lerp(_transform.position, targetPosition, 15 * Time.deltaTime);
    
        }

        inMoveReturn = inMove;

        
    }

    /// <summary>Returns the mouse/touch position. If not touching screen, keeps the inputted value.</summary>
    /// <param name="value">The value to change.</param>
    public static Vector3 GetInputPosition(this Vector3 value)
    {
        Vector3 touchPos = value;
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchPos.z = 0;    
        }
        else
        {
            if (Input.touchCount > 0)
            {

                Touch touch = Input.GetTouch(0);
                touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                touchPos.z = 0;              
            }
        }
        return touchPos;
    }

    #endregion

    #region Audio

    /// <summary>Plays a clip from a specific Audio Source.</summary>
    /// <param name="clip">The desired AudioClip to play.</param>
    /// <param name="volume">The volume at which to play the clip.</param>
    /// <param name="volumeVariability">The random range of volume to play, e.g. if .1 and "volume" is 1, volume will play at 0.9-1.1 volume.</param>
    public static void PlaySound(this AudioSource aSrc, AudioClip clip, float volume, float volumeVariability)
    {
        aSrc.clip = clip;
        float volumeMult = 1;
        if (SettingsManager.main != null)
        {
            volumeMult = SettingsManager.main.soundVolume;
        }
        aSrc.volume = (volume + Random.Range(-volumeVariability, volumeVariability))*volumeMult;
        aSrc.pitch = 1;
        aSrc.Play();

    }

    /// <summary>Plays a clip from a specific Audio Source.</summary>
    /// <param name="clip">The desired AudioClip to play.</param>
    /// <param name="volume">The volume at which to play the clip.</param>
    /// <param name="volumeVariability">The random range of volume to play, e.g. if .1 and "volume" is 1, it will play at 0.9-1.1 volume.</param>
    /// <param name="pitch">The pitch at which to play the clip.</param>
    /// <param name="pitchVariability">The random range of pitch to play, e.g. if .1 and "pitch" is 1, it will play at 0.9-1.1 pitch.</param>
    public static void PlaySound(this AudioSource aSrc, AudioClip clip, float volume, float volumeVariability, float pitch, float pitchVariability)
    {
        aSrc.clip = clip;
        float volumeMult = 1;
        if (SettingsManager.main != null)
        {
            volumeMult = SettingsManager.main.soundVolume;
        }
        aSrc.volume = (volume + Random.Range(-volumeVariability, volumeVariability)) * volumeMult;
        aSrc.pitch = pitch + Random.Range(-pitchVariability, pitchVariability);
        aSrc.Play();
        
    }

    #endregion

    #region Numerical

    /// <summary>Remaps the value from its position in a given range to another.</summary>
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    /// <summary>Gets the average value of the list.</summary>
    public static int GetAverage(this List<int> value)
    {
        int totalValue = 0;
        foreach (var singleInt in value)
        {
            totalValue += singleInt;
        }
        return Mathf.RoundToInt((float)totalValue / value.Count);
    }

    /// <summary>Gets the average value of the list.</summary>
    public static float GetAverage(this List<float> value)
    {
        float totalValue = 0;
        foreach (var singleInt in value)
        {
            totalValue += singleInt;
        }
        return totalValue / value.Count;
    }


    /// <summary>Converts a time value to a 0:00 string.</summary>
    /// <param name="showDecimal">If true, below 10 seconds will display as 8.73, e.g.</param>
    public static string TimerDisplay(this float value, bool showDecimal)
    {
        if (value < 0.02f)
        {
            
            if (showDecimal)
            {
                return "0.00";
            }
            else
            {
                return "0:00";
            }
        }
        else if (value < 10 && showDecimal)
        {
            int totalVal = Mathf.RoundToInt(value * 100);
            string ones =Mathf.FloorToInt((float)totalVal / 100f).ToString();
            int decInt = (totalVal % 100);

            string dec = (totalVal % 100).ToString();
            if (decInt < 10)
            {
                dec = "0" + decInt.ToString();
            }

            return ones + "." + dec;
        }
        else
        {
            int minutes = Mathf.FloorToInt(value / 60);
            int seconds = Mathf.CeilToInt(value % 60);
            if (seconds == 60)
            {
                minutes++;
                seconds = 00;
            }
            string secondsString = seconds.ToString();
            if (seconds < 10)
            {
                secondsString = "0" + seconds.ToString();
            }
            return minutes.ToString() + ":" + secondsString;
        }

    }

    /// <summary>Converts a decimal value to a string with a singular decimal place, e.g. 1.0787 -> 1.1</summary>
    public static string GetRoundedDecimal(this float value)
    {
        int dec = Mathf.RoundToInt((value * 10) % 1);
        return Mathf.FloorToInt(value) + "." + dec.ToString();
    }

    /// <summary>Converts an integer to a commatized string, e.g. 1034959 -> 1,034,959</summary>
    public static string WrittenNumber(this int value)
    {
        string output = "";

        int preComma = value - (value % 1000);
        int postComma = value % 1000;

        preComma /= 1000;
        preComma = preComma % 1000;

        int millions = Mathf.FloorToInt((float)value / 1000000);
        if (millions > 0)
        {
            output += millions.ToString() + ",";
            if (preComma < 100)
            {
                if (preComma < 10)
                {
                    output += "00";
                }
                else
                {
                    output += "0";
                }
            }
            output += preComma.ToString() + ",";
            if (postComma < 100)
            {
                if (postComma < 10)
                {
                    output += "00";
                }
                else
                {
                    output += "0";
                }
            }
            output += postComma.ToString();

        }
        else if (preComma > 0)
        {
            output += preComma.ToString() + ",";

            if (postComma < 100)
            {
                if (postComma < 10)
                {
                    output += "00";
                }
                else
                {
                    output += "0";
                }
            }
            output += postComma.ToString();
        }
        else
        {
            output += value.ToString();
        }

        return output;
    }
    #endregion
}
