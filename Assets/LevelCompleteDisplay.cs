using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelCompleteDisplay : MonoBehaviour
{
    [Range(0,1)]
    public float targetGlobalScale = 1;
    bool displaying;
    bool displayed;

   
    public Transform levelCompleteHolder;
    [Space]
    public SpriteRenderer background;
    public Transform levelCompleteTitle;
    [Header("Buttons")]
    public LevelCompleteButton[] buttons;
    public LevelCompleteStar[] stars;
    [Header("Character")]
    public Transform characterHolder;
    public SpriteRenderer characterOutline;
    public SpriteRenderer characterBody;
    public SpriteRenderer pupil;
    public EyeballManager eyeballManager;
    [Header("Animation Curves")]
    public AnimationCurve boxPopup;
    public AnimationCurve characterCurve;
    public AnimationCurve[] buttonCurves;
    [Space]
    [Header("Stars")]
    public AnimationCurve[] starAnimationCurves;
    [Space]
    public Color[] starOutlineColor;
    public Color[] starColor;

    // Start is called before the first frame update
    void Start()
    {
        TurnOffAll();   
    }

    private void Update()
    {
       if (displayed)
        {
            Vector3 touchP;
            bool pressed;
            ExtensionMethods.DetectTouches(out pressed, out touchP);
            if (pressed && CurrentLevelManager.main)
            {
                if (ExtensionMethods.TouchedHitbox(buttons[0].hitbox, touchP))
                {
                    TurnOffAll();
                    CurrentLevelManager.main.ResetGame();
                }
                else if (ExtensionMethods.TouchedHitbox(buttons[1].hitbox, touchP))
                {
                    displayed = false;
                    CurrentLevelManager.main.GoToMenu();
                }
                else if (ExtensionMethods.TouchedHitbox(buttons[2].hitbox, touchP))
                {
                    displayed = false;
                    CurrentLevelManager.main.GoToNextLevel();
                }
            }
        }
    }

    public void SetLevelComplete(int starsEarned)
    {
        if (!displaying)
        {
            displaying = true;
            StartCoroutine(DisplayLevelComplete(starsEarned));
        }
    }

    void TurnOffAll()
    {
        displayed = false;
        background.size = Vector2.zero;
        levelCompleteTitle.localScale = Vector2.zero;
        characterHolder.localScale = Vector2.zero;

        foreach (var button in buttons)
        {
            button.buttonTransform.localScale = Vector2.zero;

        }

        foreach (var star in stars)
        {
            star.transform.localScale = Vector2.zero;
        }
    }

    IEnumerator DisplayLevelComplete(int starsEarned)
    {

        TurnOffAll();
        levelCompleteHolder.localScale = targetGlobalScale * Vector2.one;

        float t = 0;
        Vector2 initBoxSize = Vector2.zero;
        Vector2 targetBoxSize = new Vector2(10.75f, 8.3f);
        //Vector2 targetBoxSize = new Vector2(8.62f, 9.22f);
        while (t < 1)
        {
            background.size = Vector2.Lerp(initBoxSize, targetBoxSize, boxPopup.Evaluate(t));
            if (t > .5f)
            {
                levelCompleteTitle.localScale = Vector2.one * boxPopup.Evaluate((t - .5f) * 2);
            }
            else levelCompleteTitle.localScale = Vector3.zero;


            t += Time.deltaTime / .8f;
            yield return new WaitForEndOfFrame();
        }
        background.size = targetBoxSize;
        levelCompleteTitle.localScale = Vector2.one;

        t = 0;
        while (t < .9f)
        {
            characterHolder.localScale = Vector2.one * characterCurve.Evaluate(t);
            for (int i = 0; i < 3; i++)
            {
                buttons[i].buttonTransform.localScale = Vector2.one * buttonCurves[i].Evaluate(t);
            }
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        characterHolder.localScale = Vector2.one;
        for (int i = 0; i < 3; i++)
        {
            buttons[i].buttonTransform.localScale = Vector2.one;
        }

        
        for (int i = 0; i < 3; i++)
        {
            t = 0;
            int completed = starsEarned >= i + 1 ? 1 : 0;
            stars[i].SetColor(starOutlineColor[completed], starColor[completed]);
            while (t < 1)
            {
                stars[i].transform.localScale = .6f * Vector2.one * starAnimationCurves[completed].Evaluate(t);

                t += Time.deltaTime / .3f;
                yield return new WaitForEndOfFrame();
            }
            stars[i].transform.localScale = Vector2.one * .6f;
            yield return new WaitForSeconds(.1f);
        }
        displayed = true;
    }
}

[System.Serializable]
public class LevelCompleteButton
{
    public string name;
    public Transform buttonTransform;
    public TextMeshPro buttonText;
    public BoxCollider hitbox;
    public Transform buttonIcon;
}

[System.Serializable]
public class LevelCompleteStar
{
    public Transform transform;
    public SpriteRenderer outline;
    public SpriteRenderer star;

    public void SetColor(Color outlineC, Color starC)
    {
        outline.color = outlineC;
        star.color = starC;
    }
}