using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateSpawner : MonoBehaviour
{

    public static CrateSpawner main;

    [Header("Config")]
    [Tooltip("If the player touches inside anyone of these colliders, a crate will not spawn")]
    [SerializeField] private LayerMask preventativeColliders;
    [SerializeField] private LayerMask cantPlaceColliders;
    [SerializeField] private GameObject crateObj;
    [SerializeField] private Transform crateObjectHolder;

    [Header("Initial Config (if needed)")]
    [Tooltip("Crates that are already in the scene on level load. Allows them to revert to original positions")]
    [SerializeField] private CrateInfo[] initialCrates;
    private Vector2[] initialPositions;
    private Quaternion[] initialRotation;
    private bool[] initialCratesDisabled;

    [Header("Spawn Settings")]
    [Tooltip("Time before player is able to spawn another crate")]
    [SerializeField] private float timeDelayBeforeNextCrate = .5f;
    private float timeLeftToSpawn;
    [Tooltip("Whenever a crate can be dropped, it will follow the current mouse position.")]
    [SerializeField] private bool alwaysHoldCrate = false;
    [SerializeField] [Range(0, 3)] private float heldCrateSensitivity;
    [SerializeField] private Transform breakCrateXTransform;
    private SpriteRenderer breakCrateXSR;
    private CrateInfo heldCrate;
    private Rigidbody2D heldCrateRB;
    private SpriteRenderer heldCrateSR;
    private bool holdingCrate;
    


    [Header("Input")]
    [Tooltip("Controls how accurately crate follows mouse, only in 'Always Hold Crate' mode.")]
    [Range(0.01f, 1)] [SerializeField] private float sensitivityLerp = .5f;

    [Header("Crate Outline")]
    [SerializeField] private SpriteRenderer crateOutlineSR;
    [SerializeField] private Sprite[] crateOutlineSprites;

    #region Internal Variables
    List<CrateInfo> allDroppedCrates;

    //Input
    [HideInInspector] public Vector3 touchPos;
    [HideInInspector] public Vector3 mousePos;

    [HideInInspector] public bool dropDisabled;

    private bool pressed;
    private const float touchSens = .6f;

    #endregion


    //Create singleton
    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        SaveInitialCratePositions();

        if (breakCrateXTransform) breakCrateXSR = breakCrateXTransform.GetComponent<SpriteRenderer>();
        breakCrateXSR.enabled = false;
    }

    private void Update()
    {
        DetectTouches();
        UpdateTimers();

        if (alwaysHoldCrate) AutoSpawnCrateInput();
        else TouchToSpawnCrateInput();
    }

    /// <summary>
    /// Initializes the positions of any crates already in the scene.
    /// </summary>
    void SaveInitialCratePositions()
    {
        allDroppedCrates = new List<CrateInfo>();

        initialPositions = new Vector2[initialCrates.Length];
        initialRotation = new Quaternion[initialCrates.Length];
        initialRotation = new Quaternion[initialCrates.Length];
        initialCratesDisabled = new bool[initialCrates.Length];
        for (int i = 0; i < initialCrates.Length; i++)
        {
            initialPositions[i] = initialCrates[i].transform.position;
            initialRotation[i] = initialCrates[i].transform.rotation;
        }
    }

    /// <summary>
    /// Checks for crate spawning and breaking, as well as gets any input.
    /// </summary>
    void DetectTouches()
    {
        mousePos = ExtensionMethods.GetInputPosition(mousePos);
        mousePos.z = 0;

        ExtensionMethods.DetectTouches(out pressed, out touchPos);
        touchPos.z = 0;
    }


    /// <summary>
    /// Crate spawning and breaking if the crates do not auto-spawn.
    /// </summary>
    void TouchToSpawnCrateInput()
    {
        if (pressed && !dropDisabled)
        {
            bool canSpawn = ValidTouchPosition(touchPos);
            bool isInitialCrate = false;
            CrateInfo brokenCrate = TouchesCrate(touchPos, ref isInitialCrate);
            if (brokenCrate)
            {
                BreakCrate(brokenCrate, isInitialCrate);
                return;
            }
            else if (canSpawn)
            {
                TrySpawnCrate();
            }
        }
    }

    void AutoSpawnCrateInput()
    {
        if (heldCrate != null)
        {
            TrackHeldCrate();
        }
        else
        {
            TryHoldCrate();
        }

        bool isInitialCrate = false;
        CrateInfo touchedCrate = TouchesCrate(mousePos, ref isInitialCrate);
        bool validPosition = ValidPlacePosition();

        if (touchedCrate)
        {
            if (heldCrate) heldCrateSR.enabled = false;
            breakCrateXSR.enabled = true;
        }
        else if (validPosition)
        {
            if (heldCrate) heldCrateSR.enabled = true;
            breakCrateXSR.enabled = false;
        }
        else
        {
            if (heldCrate) heldCrateSR.enabled = false;
            breakCrateXSR.enabled = false;
        }

        if (pressed && !dropDisabled)
        {
            if (touchedCrate) BreakCrate(touchedCrate, isInitialCrate);
            else if (validPosition) TryDropHeldCrate();
        }
    }

    private void TryHoldCrate()
    {
        if (timeLeftToSpawn > 0) return;
        if (!ValidTouchPosition(mousePos)) return;
        timeLeftToSpawn = timeDelayBeforeNextCrate;
        holdingCrate = true;
        GameObject newCrate = Instantiate(crateObj, mousePos, Quaternion.identity, crateObjectHolder);
        newCrate.transform.position = mousePos;


        heldCrate = newCrate.GetComponent<CrateInfo>();
        heldCrateRB = heldCrate.GetComponent<Rigidbody2D>();
        heldCrateRB.angularDrag = 100;
        heldCrateRB.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        heldCrateSR = heldCrate.GetComponentInChildren<SpriteRenderer>();
        heldCrateSR.color = new Color(1, 1, 1, .4f);
        heldCrate.gameObject.layer = 13; ///Sets the layer to CratePieces to avoid colliding with player or anything important (besides walls).
    }

    void TrackHeldCrate()
    {
        heldCrateRB.velocity = ((Vector2)mousePos - heldCrateRB.position) * heldCrateSensitivity * 5;
        breakCrateXTransform.transform.position = mousePos;
    }

    void TryDropHeldCrate()
    {
        if (timeLeftToSpawn > 0) return;
        if (!heldCrate) return;
        heldCrate.transform.parent = crateObjectHolder;
        heldCrate.gameObject.layer = 9; ///Sets layer back to being with other crates.
        allDroppedCrates.Add(heldCrate);
        heldCrateSR.color = Color.white;
        heldCrateRB.angularDrag = 0.05f;

        heldCrate = null;
        heldCrateRB = null;
        heldCrateSR = null;

    }

    #region Crate Breaking and Instantiating

    /// <summary>
    /// Checks whether the current mouse position is over a crate or not.
    /// </summary>
    /// <param name="touchP">Touch/input position.</param>
    /// <param name="isInitialCrate">Returns whether the crate is initially in the scene or not.</param>
    /// <returns></returns>
    CrateInfo TouchesCrate(Vector3 touchP, ref bool isInitialCrate)
    {
        for (int i = 0; i < allDroppedCrates.Count; i++)
        {
            if (!allDroppedCrates[i]) continue;
            if (allDroppedCrates[i].rb.OverlapPoint(touchP))
            {
                isInitialCrate = false;
                return allDroppedCrates[i];
            }
        }
        for (int i = 0; i < initialCrates.Length; i++)
        {
            if (initialCratesDisabled[i]) continue;
            if (initialCrates[i].rb.OverlapPoint(touchP))
            {
                isInitialCrate = true;
                return initialCrates[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Breaks the desired crate.
    /// </summary>
    /// <param name="crate">Crate to break.</param>
    /// <param name="isInitialCrate">True if crate is initially in scene.</param>
    void BreakCrate(CrateInfo crate, bool isInitialCrate)
    {
        crate.BreakCrate(false);

        if (!isInitialCrate)
        {
            Destroy(crate.gameObject);
            allDroppedCrates.Remove(crate);
        }
        else
        {
            DisableCrate(crate);
        }


        //Sound
        if (SFXManager.main)
        {
            SFXManager.main.CrateBreakSound();
        }
    }

    /// <summary>
    /// Breaks the desired crate.
    /// </summary>
    /// <param name="crate">Crate to break.</param>
    public void BreakCrate(CrateInfo crate)
    {
        bool initCrate = true;
        for (int i = 0; i < allDroppedCrates.Count; i++)
        {
            if (allDroppedCrates[i] == crate)
            {
                initCrate = true;
                break;
            }
        }

        crate.BreakCrate(false);

        if (initCrate)
        {
            Destroy(crate.gameObject);
            allDroppedCrates.Remove(crate);
        }
        else
        {
            DisableCrate(crate);
        }

        //Sound
        if (SFXManager.main)
        {
            SFXManager.main.CrateBreakSound();
        }
    }

    /// <summary>
    /// Gets the index of a crate in its specified array.
    /// </summary>
    /// <param name="crate">Crate to get the index of.</param>
    /// <returns></returns>
    private int GetCrateIndex(CrateInfo crate)
    {
        for (int i = 0; i < allDroppedCrates.Count; i++)
        {
            if (crate == allDroppedCrates[i]) return i;
        }
        for (int i = 0; i < initialCrates.Length; i++)
        {
            if (crate == initialCrates[i]) return i;
        }
        return -1;
    }

    /// <summary>
    /// Enables an initial crate and puts it back to its original position.
    /// </summary>
    /// <param name="crate">Crate to enable.</param>
    private void EnableCrate(CrateInfo crate)
    {
        crate.gameObject.SetActive(true);
        int index = GetCrateIndex(crate);
        initialCratesDisabled[index] = false;
        crate.transform.position = initialPositions[index];
        crate.transform.rotation = initialRotation[index];
    }

    /// <summary>
    /// Disables an initial crate and puts it back to its original position.
    /// </summary>
    /// <param name="crate">Crate to disable.</param>
    private void DisableCrate(CrateInfo crate)
    {
        crate.gameObject.SetActive(false);
        initialCratesDisabled[GetCrateIndex(crate)] = true;
    }

    /// <summary>
    /// Tries to spawn a crate at mouse position.
    /// </summary>
    private void TrySpawnCrate()
    {
        if (timeLeftToSpawn > 0) return;
        if (!ValidTouchPosition(mousePos)) return;
        timeLeftToSpawn = timeDelayBeforeNextCrate;

        GameObject newCrate = Instantiate(crateObj, mousePos, Quaternion.identity, crateObjectHolder);
        newCrate.transform.position = mousePos;

        allDroppedCrates.Add(newCrate.GetComponent<CrateInfo>());
    }

    /// <summary>
    /// Resets all initial crates and clears all dropped crates.
    /// </summary>
    public void ResetCrates()
    {
        //Reset initial crates
        for (int i = 0; i < initialCrates.Length; i++)
        {
            EnableCrate(initialCrates[i]);
        }

        foreach (var crate in allDroppedCrates)
        {
            crate.BreakCrate(true);
        }
        foreach (Transform child in crateObjectHolder)
        {

            allDroppedCrates = new List<CrateInfo>();
            Destroy(child.gameObject);
        }
    }

    #endregion

    #region Helper Functions

    /// <summary>
    /// Checks whether the current touch position would spawn a crate inside a wall.
    /// </summary>
    /// <param name="touchP">Touch/input position.</param>
    /// <returns></returns>
    bool ValidTouchPosition(Vector3 touchP)
    {
        return !Physics2D.OverlapCircle(touchP, .1f, preventativeColliders);
    }

    bool ValidPlacePosition()
    {
        if (!heldCrate) return false;
        Collider2D[] results = new Collider2D[1];
        return !Physics2D.OverlapCircle(heldCrateRB.position, .45f, cantPlaceColliders);
    }

    /// <summary>
    /// Updates any timers to check if crate can spawn.
    /// </summary>
    void UpdateTimers()
    {
        timeLeftToSpawn -= Time.deltaTime;
    }

    #endregion

    #region Various Functionality
    public void LaunchCrates(Vector3 originPosition, float power)
    {
        //Launch Dropped Crates
        foreach (CrateInfo crateInfo in allDroppedCrates)
        {
            LaunchCrate(crateInfo, originPosition, power);
        }

        for (int i = 0; i < initialCrates.Length; i++)
        {
            if (initialCratesDisabled[i]) continue;
            LaunchCrate(initialCrates[i], originPosition, power);
        }

        //Launch Player
        Vector2 distPL = PlayerManager.main.transform.position - originPosition;
        PlayerManager.main.rb.AddForce(power * distPL.normalized / Mathf.Pow(distPL.magnitude, 2f), ForceMode2D.Impulse);
    }

    private void LaunchCrate(CrateInfo crate, Vector3 originPosition, float power)
    {
        Vector2 distCR = crate.transform.position - originPosition;
        if (Physics.Linecast(crate.transform.position, originPosition, out RaycastHit hit))
        {
            if (hit.collider.tag == "tnt")
            {
                crate.rb.AddForce(power * distCR.normalized / Mathf.Pow(distCR.magnitude, 2), ForceMode2D.Impulse);
            }
        }
    }
    #endregion
}
