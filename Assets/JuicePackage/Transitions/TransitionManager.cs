using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager main;

    

    public Transform transitionQuad;
    public Material transitionMat;

    private Texture2D _screenShot;
    [Space]
    public float transitionTime;
    public AnimationCurve transitionCurve;
    public Color[] defaultColors;
    public Color[] iceColors;
    bool inChange;

    private void Awake()
    {

        if (main != null && main != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            main = this;
            DontDestroyOnLoad(main);
        }
    }


    // Update is called once per frame
    void Update()
    {
        UpdateTransitionSize();
        UpdateMaterial();

        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(StartTransition());
        }
    }


    public void UpdateTransitionSize()
    { 
        float quadHeight = Camera.main.orthographicSize * 2f;
        float quadWidth = quadHeight * Screen.width / Screen.height;
        transitionQuad.localScale = new Vector3(quadWidth, quadHeight, 1);
        Vector3 tPos = Camera.main.transform.position;
        tPos.z = -9f;
        transitionQuad.position = tPos;
    }

    public void UpdateMaterial()
    {
        float aspectRatio = Screen.width / (float)Screen.height;
        transitionMat.SetVector("_NoiseTiling", new Vector4(2.5f * aspectRatio, 2.5f , 0, 0));
    }

    public void UpdatePalette()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        print("Build no" + currentScene);
        Color[] colors;
        if (currentScene<9 || currentScene == 14)
        {
            print("SETTING DEFAULT");
            colors = defaultColors;
        }
        else
        {
            colors = iceColors;
        }
        transitionMat.SetColor("_TransitionColor1", colors[0]);
        transitionMat.SetColor("_TransitionColor2", colors[1]);
        transitionMat.SetColor("_TransitionColor3", colors[2]);
    }

    public IEnumerator StartTransition()
    {
        UpdatePalette();
        transitionMat.SetFloat("_TransitionValue", 0);
        inChange = true;
        int resWidth = Screen.width;
        int resHeight = Screen.height;
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        Camera.main.targetTexture = rt;
        _screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        Camera.main.Render();
        RenderTexture.active = rt;
        _screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        _screenShot.Apply();
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        transitionMat.SetTexture("_MainTexture", _screenShot);
        transitionMat.SetFloat("_TransitionValue", 1);

        float t = 0;
        float fadeVal = 1;
        while (t < transitionTime)
        {
            t += Time.deltaTime;
            fadeVal = t.Remap(0, transitionTime, 1, 0);
            fadeVal = transitionCurve.Evaluate(fadeVal);
            transitionMat.SetFloat("_TransitionValue", fadeVal);
            yield return new WaitForEndOfFrame();
        }
        transitionMat.SetFloat("_TransitionValue", 0);
        inChange = false;
    }

    public void Transition()
    {
        UpdatePalette();
            if (!inChange)
            {
                print("CHANGE");
                StartCoroutine(StartTransition());
            }
            else
            {
                StopCoroutine(StartTransition());

                StartCoroutine(StartTransition());
            }
    }
}
