using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EndScene : MonoBehaviour
{

    public ButtonScript bs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (bs.buttonPressed)
        {
            if (TransitionManager.main != null)
            {
                TransitionManager.main.Transition();
            }
            SceneManager.LoadScene(0);
        }
    }
}
