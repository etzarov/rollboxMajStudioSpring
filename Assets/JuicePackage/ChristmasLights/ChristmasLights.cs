using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChristmasLights : MonoBehaviour
{

    [Header("Bezier")]
    public Transform startPoint;
    public Transform endPoint;
    public Transform bezierPoint;
    [Space]
    public int midPoints;
    [Header("Visual")]
    public LineRenderer lr;
    public GameObject christmasLightObj;
    [Space]
    public int numberOfLights = 10;
    public float outsideOffset;
    [Space]
    public float yOffset;
    [Space]
    [Range(0f,3f)]
    public float lightSpeed;
    public Color[] allColors;
    [Space]
    public float ropeBobAmplitude;
    public float ropeBobFrequency;

    List<Transform> allLights;
    List<SpriteRenderer> allSRs;

    float initY;


    // Start is called before the first frame update
    void Start()
    {
        initY = bezierPoint.position.y;
        InstantiateLights();
    }

    private void OnDrawGizmos()
    {
        Vector2 previousPos = startPoint.position;
        for (float i = .05f; i < 1; i += .05f)
        {
            Vector2 targetPos = GetBezierPoint(i);
            Gizmos.DrawLine(previousPos, targetPos);
            previousPos = targetPos;
        }

    }

    void Update()
    {
        UpdateLightColor();
        UpdateLightPosition();
    }

    Vector2 GetBezierPoint(float t)
    {
        t = Mathf.Clamp01(t);
        return (Mathf.Pow(1 - t, 2) * startPoint.position) + (2*t*(1 - t) * bezierPoint.position) + (Mathf.Pow(t, 2) * endPoint.position);
    }

    void InstantiateLights()
    {
        allLights = new List<Transform>();
        allSRs = new List<SpriteRenderer>();
        lr.positionCount = 21;
        for (int i = 0; i < lr.positionCount; i++)
        {
            float t = i / 20f;
            lr.SetPosition(i, GetBezierPoint(t));
        }

        float startT = outsideOffset;
        float stepC = (1f - outsideOffset * 2f) / (float)(numberOfLights-1);
        for (int i = 0; i < numberOfLights; i++)
        {
            float iValue = startT + (float)i * stepC;
            Vector3 position = GetBezierPoint(iValue);
            Vector3 rotation = Vector3.zero;
            rotation.z = Random.Range(-10, 10);
            position.y += yOffset;
            GameObject lightObj = Instantiate(christmasLightObj, position, Quaternion.Euler(rotation), transform);
            print("i:" + i + " || t:" + iValue + " || Calculated:" + (i/numberOfLights));
            allLights.Add(lightObj.transform);

            allSRs.Add(lightObj.GetComponent<SpriteRenderer>());
        }
    }

    void UpdateLightColor()
    {
        float t = Time.time*lightSpeed;
        for (int i = 0; i < allLights.Count; i++)
        {
            int colorIndex = Mathf.FloorToInt((t + i) % allColors.Length);
            allSRs[i].color = allColors[colorIndex];
        }
    }

    void UpdateLightPosition()
    {
        Vector3 targetPos = bezierPoint.position;
        targetPos.y = initY + Mathf.Sin(Time.time * ropeBobFrequency) * ropeBobAmplitude;
        bezierPoint.position = targetPos;
        float startT = outsideOffset;
        float stepC = (1f - outsideOffset * 2f) / (float)(numberOfLights - 1);
        for (int i = 0; i < allLights.Count; i++)
        {
            float iValue = startT + (float)i * stepC;
            Vector3 position = GetBezierPoint(iValue);
            position.y += yOffset;
            allLights[i].position = position;
        }

        for (int i = 0; i < lr.positionCount; i++)
        {
            float t = i / 20f;
            lr.SetPosition(i, GetBezierPoint(t));
        }
    }

    Vector2 GetMousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

}
