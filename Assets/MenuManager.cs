using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager main;
    public static bool hasLoadedOnce;
    public static Vector3 cameraPos;

    public ButtonScript settingsButton;
    public ButtonScript playButton;
    public ButtonScript customizeButton;

    public MenuMusicManager menuMusicManager;

    public enum MenuState
    {
        MainScreen,
        Settings,
        Customize,
        LevelSelect
    }

    public MenuState currentState;
    MenuState targetState;
    public bool inStateChange;
    [Space]
    public float transitionTime;
    public AnimationCurve changeCurve;
    [Header("Level Management")]
    public Transform levelSelectHolder;
    public BoxCollider levelSliderHitbox;
    public Transform levelSliderKnob;
    public SpriteRenderer levelSliderKnobSprite;
    public Vector2 cameraYBounds;
    public float scrollSpeed;
    public float targetLevelPos;
    bool adjustingLevelPosition;
    [Space]
    public BoxCollider goBackLevelSelect;

    [Header("Options")]
    [Space(20)]
    public Transform settingsHolder;
    public Transform customizeHolder;
    [Space]
    [Header("Settings Management")]
    public BoxCollider goBackHitbox;
    [Space]
    public BoxCollider sfxHitbox;
    public BoxCollider musicHitbox;
    bool justPressed;
    bool adjustingSFX;
    bool adjustingMusic;
    [Space(20)]
    public Transform sfxKnob;
    public Transform musicKnob;
    public SpriteRenderer sfxKnobSprite;
    public SpriteRenderer musicKnobSprite;
    [Space]
    public Color selectColor;
    public Color defaultColor;
    [Header("Customize Management")]
    public PlayerStyleCustomizeButton[] styleCustomButtons;
    public PlayerStyleManager psm;
    public float tack;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {
        targetState = currentState;
        SettingsLoad();


        //load back to levelMap
        if (hasLoadedOnce)
        {
            currentState = MenuState.LevelSelect;
            targetState = MenuState.LevelSelect;
            menuMusicManager.currentHeight = cameraPos.y;
            Camera.main.transform.position = cameraPos;
            
        }
        hasLoadedOnce = true;
    }

    void Update()
    {
        cameraPos = Camera.main.transform.position;
        if (!inStateChange)
        {
            if (settingsButton.buttonPressed)
            {
                CrateSpawner.main.dropDisabled = true;
                targetState = MenuState.Settings;
            }
            else if (playButton.buttonPressed)
            {
                CrateSpawner.main.dropDisabled = true;
                targetState = MenuState.LevelSelect;
            }
            else if (customizeButton.buttonPressed)
            {
                CrateSpawner.main.dropDisabled = true;

                targetState = MenuState.Customize;
            }
        }

        
        if (!inStateChange && currentState!=targetState)
        {
            StartCoroutine(ChangeMenuState(targetState));
        }


        if (currentState == MenuState.Settings)
        {
            SettingsUpdate();
        }
        else if (currentState == MenuState.LevelSelect)
        {
            LevelSelectUpdate();
        }
        Vector3 lsHolderPos = levelSelectHolder.position;
        lsHolderPos.y = Camera.main.transform.position.y;
        levelSelectHolder.position = lsHolderPos;

        if (currentState == MenuState.Settings || currentState == MenuState.Customize)
        {
            if (ExtensionMethods.TouchedHitbox(goBackHitbox))
            {
                targetState = MenuState.MainScreen;
                SFXManager.main.PlaySelectSound(SFXManager.SelectSound.GoBack,1);
            }
        }
    }


    public IEnumerator ChangeMenuState(MenuState newState)
    {
        if (newState == MenuState.Settings)
        {
            settingsHolder.gameObject.SetActive(true);
            customizeHolder.gameObject.SetActive(false);
        }
        else if (newState == MenuState.Customize)
        {
            settingsHolder.gameObject.SetActive(false);
            customizeHolder.gameObject.SetActive(true);
        }
        else if (newState == MenuState.LevelSelect)
        {
            LevelSelectManager.main.LoadData(true);
            menuMusicManager.MoveToLevelMap();
        }
        else
        {
            menuMusicManager.MoveToMenu();
        }
        


        SettingsLoad();

        inStateChange = true;
        MenuState previousState = currentState;
        currentState = newState;

        if (previousState == MenuState.LevelSelect)
        {
            if (Camera.main.transform.position.y > .05f)
            {
                while (Camera.main.transform.position.y > .05f)
                {
                    Vector3 newPos = Camera.main.transform.position;
                    float changeAmount = newPos.y * .05f;
                    changeAmount = Mathf.Clamp(changeAmount, 1f*Time.deltaTime, 20f*Time.deltaTime);
                    newPos.y = Mathf.Max(newPos.y - changeAmount, 0);
                    Camera.main.transform.position = newPos;
                    UpdateLevelSelectKnob();
                    menuMusicManager.currentHeight = newPos.y;
                    yield return new WaitForEndOfFrame();

                }
                Vector3 nPos = Camera.main.transform.position;
                nPos.y = 0;
                nPos.x = 20;
                
                Camera.main.transform.position = nPos;
                UpdateLevelSelectKnob();
            }
        }


        Vector3 currentCameraPosition = Camera.main.transform.position;
        Vector3 targetCameraPos = Vector2.zero;

        targetCameraPos.z = -10;
        if (newState == MenuState.Settings || newState == MenuState.Customize)
        {
            targetCameraPos.y = -7.5f;

            foreach (var styleButton in styleCustomButtons)
            {
                styleButton.UpdateStyleCustomization();
            }

        }
        if (newState == MenuState.LevelSelect)
        {
            targetCameraPos.x = 20;
        }

        float t = 0;
        while (t < transitionTime)
        {
            UpdateLevelSelectKnob();
            float aEval = changeCurve.Evaluate(t / transitionTime);
            Camera.main.transform.position = Vector3.Lerp(currentCameraPosition, targetCameraPos, aEval);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (CrateSpawner.main)
        {
            CrateSpawner.main.ResetCrates();
        }

        Camera.main.transform.position = targetCameraPos;
        inStateChange = false;
        if (newState == MenuState.MainScreen)
        {
            CrateSpawner.main.dropDisabled = false;
        }
        yield return null;
    }

    public void SettingsUpdate()
    {

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
            touchPos.z = 0;
            if (pressedFrame)
            {

                if (ExtensionMethods.TouchedHitbox(sfxHitbox, touchPos))
                {
                    justPressed = true;
                    adjustingSFX = true;
                    SFXManager.main.PlaySelectSound(SFXManager.SelectSound.DefaultClick,1);
                    //MMVibrationManager.TransientHaptic(.2f, .25f);
                }
                else if (ExtensionMethods.TouchedHitbox(musicHitbox, touchPos))
                {
                    justPressed = true;
                    adjustingMusic = true;
                    SFXManager.main.PlaySelectSound(SFXManager.SelectSound.DefaultClick,1);
                }

            }

            if (adjustingSFX)
            {
                float targetX = touchPos.x;
                Vector3 newPos = sfxKnob.transform.position;
                targetX = Mathf.Clamp(targetX, -4.5f, 4.5f);
                newPos.x = targetX;
                sfxKnob.transform.position = newPos;
                SettingsManager.main.soundVolume = targetX.Remap(-4.5f, 4.5f, 0, 1);
                sfxKnobSprite.color = selectColor;
            }

            if (adjustingMusic)
            {
                float targetX = touchPos.x;
                Vector3 newPos = musicKnob.transform.position;
                targetX = Mathf.Clamp(targetX, -4.5f, 4.5f);
                newPos.x = targetX;
                musicKnob.transform.position = newPos;
                SettingsManager.main.musicVolume = targetX.Remap(-4.5f, 4.5f, 0, 1);
                
                musicKnobSprite.color = selectColor;
            }



        }
        else
        {
            if (justPressed)
            {
                justPressed = false;
                SFXManager.main.PlaySelectSound(SFXManager.SelectSound.DefaultClick,1);
                //MMVibrationManager.TransientHaptic(.1f, .1f);

                MusicManager.main.UpdateMusicVolume();
                SettingsManager.main.SaveSettings();

                
            }
            sfxKnobSprite.color = defaultColor;
            adjustingSFX = false;
            musicKnobSprite.color = defaultColor;
            adjustingMusic = false;
        }

        SFXManager.main.UpdateVolume();
        MusicManager.main.UpdateMusicVolume();
    }
    void SettingsLoad()
    {
        float targetSoundX = SettingsManager.main.soundVolume.Remap(0, 1, -4.5f, 4.5f);
        float targetMusicX = SettingsManager.main.musicVolume.Remap(0, 1, -4.5f, 4.5f);

        Vector3 sPos = sfxKnob.transform.position;
        sPos.x = targetSoundX;
        sfxKnob.transform.position = sPos;

        Vector3 mPos = musicKnob.transform.position;
        mPos.x = targetMusicX;
        musicKnob.transform.position = mPos;
    }


    void LevelSelectUpdate()
    {
        #region Update Slider
        if (!inStateChange)
        {
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
                touchPos.z = 0;
                if (pressedFrame)
                {

                    if (ExtensionMethods.TouchedHitbox(levelSliderHitbox, touchPos))
                    {
                        justPressed = true;
                        adjustingLevelPosition = true;
                        SFXManager.main.PlaySelectSound(SFXManager.SelectSound.DefaultClick, 1);
                        //MMVibrationManager.TransientHaptic(.2f, .25f);
                    }

                }

                if (adjustingLevelPosition)
                {
                    float targetY= touchPos.y;
                    Vector3 newPos = levelSliderKnob.transform.localPosition;
                    targetY = Mathf.Clamp(targetY, levelSelectHolder.transform.position.y-4f, levelSelectHolder.transform.position.y+4f);
                    float localTargetY = targetY - levelSelectHolder.transform.position.y;
                    newPos.y = localTargetY;
                    levelSliderKnob.transform.localPosition = newPos;
                    targetLevelPos = targetY.Remap(levelSelectHolder.transform.position.y - 4f, levelSelectHolder.transform.position.y+4f, cameraYBounds.x, cameraYBounds.y);
                    levelSliderKnobSprite.color = selectColor;
                }



            }
            else
            {
                if (justPressed)
                {
                    justPressed = false;
                    SFXManager.main.PlaySelectSound(SFXManager.SelectSound.DefaultClick, 1);
                    
                }
                adjustingLevelPosition = false;
                levelSliderKnobSprite.color = defaultColor;

            }

            if (Mathf.Abs(Input.mouseScrollDelta.y) > .05f)
            {
                float targetY = levelSliderKnob.transform.position.y + (Input.mouseScrollDelta.y*scrollSpeed);
                Vector3 newPos = levelSliderKnob.transform.localPosition;
                targetY = Mathf.Clamp(targetY, levelSelectHolder.transform.position.y - 4f, levelSelectHolder.transform.position.y + 4f);
                float localTargetY = targetY - levelSelectHolder.transform.position.y;
                newPos.y = localTargetY;
                levelSliderKnob.transform.localPosition = newPos;
                targetLevelPos = targetY.Remap(levelSelectHolder.transform.position.y - 4f, levelSelectHolder.transform.position.y + 4f, cameraYBounds.x, cameraYBounds.y);
            }

            if (ExtensionMethods.TouchedHitbox(goBackLevelSelect))
            {
                targetState = MenuState.MainScreen;
                SFXManager.main.PlaySelectSound(SFXManager.SelectSound.GoBack, 1);
            }
        }
        #endregion
        Vector3 tPos = Camera.main.transform.position;
        tPos.y = Mathf.Lerp(tPos.y, targetLevelPos, .1f);
        if (!inStateChange)
        {
            menuMusicManager.currentHeight = tPos.y;
        }
        Camera.main.transform.position = tPos;


        
    }
    void UpdateLevelSelectKnob()
    {
        float targetY = Camera.main.transform.position.y.Remap(cameraYBounds.x, cameraYBounds.y, -4, 4);
        Vector3 lPos = levelSliderKnob.localPosition;
        lPos.y = targetY;
        levelSliderKnob.localPosition = lPos;
        targetLevelPos = targetY.Remap(levelSelectHolder.transform.position.y - 4f, levelSelectHolder.transform.position.y + 4f, cameraYBounds.x, cameraYBounds.y);
    }

    public void SetPlayerStyle(PlayerStyle style)
    {
        print("updateStyle");
        psm.UpdateStyle(style);
        SettingsManager.main.playerStyle = style;
        SettingsManager.main.SaveSettings();
    }

}
