using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPool : MonoBehaviour
{
    public Material waterMat;
    [Space]
    public float waterWidth;
    [Header("Water Display Settings")]
    [Tooltip("The layers that when raycasted upon, the water will stop at.")]
    public LayerMask waterCollisionMask;
    [Tooltip("The distance between each vertice of the water mesh calculated.")]
    public float waterDensity;
    [Tooltip("Extra Y Offset to move the water down. Prevents crates/ball from clipping through when sunk at bottom.")]
    [Range(-1,0)]
    public float waterYOffset;
    [Range(0, 1)]
    public float outlineOpacity;
    [Space]
    [Tooltip("The distance from the water top an outline will draw.")]
    [Range(-.1f,0)]
    public float topOutlineOffset;
    [Header("Wave Settings")]
    [Range(0,2)]
    public float waveAmplitude;
    [Range(0,2)]
    public float waveSpeed;
    [Range(0,5)]
    public float waveDensity;
    [Header("Button Config")]
    public bool usesButtons;
    [Space]
    public bool isActivated;
    public float startY;
    public float activatedY;
    [Space]
    public float changeSpeed;
    [Header("Water Flow")]
    [Tooltip("Directional Pushing of the Water")]
    public float waterFlow;
    public float bubbleFlowMultiplier;
    [Header("Extra Components / IGNORE")]
    public BuoyancyEffector2D regBuoyancy;
    public BuoyancyEffector2D playerBuoyancy;




    float defaultY;

    MeshFilter mf;
    MeshRenderer mr;

    Mesh mesh;

    float screenLeftX;
    float screenBottomY;
    float screenRightX;
    float screenTopY;


    void Start()
    {
        GetComponents();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScreen();
        UpdateMesh();

        if (usesButtons)
        {
            Vector2 pos = transform.position;
            float targetY = transform.position.y;
            if (isActivated)
            {
                targetY = targetY + (activatedY - targetY) * changeSpeed*Time.deltaTime;
            }
            else
            {
                targetY = targetY + (startY - targetY) * changeSpeed*Time.deltaTime;
            }
            pos.y = targetY;
            transform.position = pos;
        }

    }

    private void OnDrawGizmos()
    {
        UpdateScreen();
        Vector3 newPos = transform.position;
        newPos.x += waterWidth;
        Gizmos.DrawLine(transform.position, newPos);
        Gizmos.DrawSphere(transform.position, .3f);
        Gizmos.DrawSphere(newPos, .3f);
        Gizmos.DrawSphere(new Vector2(screenLeftX,screenBottomY), .3f);
        Gizmos.DrawSphere(new Vector2(screenRightX,screenTopY), .3f);

        UpdateMesh();
    }

    void UpdateScreen()
    {
        screenLeftX = Camera.main.transform.position.x - Screen.width / 2f;
        screenBottomY = Camera.main.transform.position.y - Screen.height / 2f;
        screenRightX = screenLeftX + Screen.width;
        screenTopY = screenBottomY + Screen.height;
    }

    void UpdateMesh()
    {
        GetComponents();
        GenerateMesh();

        regBuoyancy.flowMagnitude = waterFlow * 20;
        playerBuoyancy.flowMagnitude = waterFlow * 5;
    }

    void GetComponents()
    {

        if (mf == null)
        {
            mf = GetComponent<MeshFilter>();
        }

        if (mr == null)
        {
            mr = GetComponent<MeshRenderer>();
        }

        mesh = new Mesh();


    }

    void GenerateMesh()
    {


        if (waterWidth < .05f)
        {
            return;
        }

        float defX = transform.position.x;

            defaultY = transform.position.y;



        Vector3 _uvP = Camera.main.WorldToScreenPoint(new Vector3(0, defaultY));
        float uv_tY = _uvP.y.Remap(0, Screen.height, 0, 1);

        waterMat.SetFloat("_WaterLine", uv_tY + topOutlineOffset);
        waterMat.SetFloat("_WaterFlowX", -waterFlow*bubbleFlowMultiplier);
        waterMat.SetFloat("_OutlineOpacity", outlineOpacity);
        mr.material = waterMat;


        if (waterDensity < .05f)
        {
            waterDensity = .05f;
        }   

        int surfacePoints = Mathf.Max(Mathf.FloorToInt(waterWidth / waterDensity), 2);
        Vector3[] vertices = new Vector3[surfacePoints * 2];
        Vector3[] normals = new Vector3[surfacePoints * 2];

        Vector2[] uvs = new Vector2[surfacePoints * 2];

        int[] tris = new int[(surfacePoints - 1) * 6];

        mesh.vertices = new Vector3[vertices.Length];
        mesh.normals = new Vector3[normals.Length];
        mesh.uv = new Vector2[uvs.Length];
        mesh.triangles = new int[tris.Length];

        for (int i = 0; i < surfacePoints; i++)
        {
            int surfaceIndex = i * 2;
            int bottomIndex = i * 2 + 1;

            
            float targetX = ((float)i).Remap(0, surfacePoints-1, 0, waterWidth);
            float targetWorldX = ((float)i).Remap(0, surfacePoints-1, defX, defX + waterWidth);

            Vector3 defaultSurfacePos = new Vector3(targetWorldX, defaultY);
            float bottomY = defaultY;
            RaycastHit2D bottomHit = Physics2D.Raycast(defaultSurfacePos, Vector2.down, 20, waterCollisionMask, -10, 10);
            if (bottomHit.distance > 19)
            {
                bottomY = -10;
            }
            else
            {
                bottomY = bottomHit.point.y - defaultY + waterYOffset;
            }

            float topY = Mathf.Max(GetWaterOffset(targetWorldX),bottomY);

            vertices[surfaceIndex] = new Vector3(targetX, topY,-1);
            vertices[bottomIndex] = new Vector3(targetX, bottomY,-1);

            normals[surfaceIndex] = Vector3.forward;
            normals[bottomIndex] = Vector3.forward;


            Vector3 uvPos = Camera.main.WorldToScreenPoint(new Vector3(targetWorldX, defaultY));
            Vector3 uvBPos = Camera.main.WorldToScreenPoint(new Vector3(targetWorldX, defaultY+bottomY));

            float uv_x = uvPos.x.Remap(0, Screen.width, 0, 1);
            float uv_topY = uvPos.y.Remap(0, Screen.height, 0, 1);
            float uv_bottomY = uvBPos.y.Remap(0, Screen.height, 0, 1);

            uvs[surfaceIndex] = new Vector2(uv_x, uv_topY);
            uvs[bottomIndex] = new Vector2(uv_x, uv_bottomY);

            if (i != 0)
            {
                int initialTri = (i - 1) * 6;
                int uLeft = (i - 1) * 2;
                tris[initialTri] = uLeft;
                tris[initialTri + 1] = uLeft + 2;
                tris[initialTri + 2] = uLeft + 1;
                tris[initialTri + 3] = uLeft + 1;
                tris[initialTri + 4] = uLeft + 2;
                tris[initialTri + 5] = uLeft + 3;
            }


        }

        mesh.vertices = vertices;
        mesh.normals = normals;

        mesh.uv = uvs;
        mesh.triangles = tris;

        mf.mesh = mesh;
    }


    float GetWaterOffset(float worldX)
    {
        float defValue = Mathf.Abs(waterFlow) < .2f ? 1 : waterFlow*.7f;
        defValue *= Time.time / waveSpeed;
        return (Mathf.Sin(waveDensity*worldX - defValue) - Mathf.Cos(waveDensity*worldX / 2 - defValue)) * waveAmplitude;
    }
}
