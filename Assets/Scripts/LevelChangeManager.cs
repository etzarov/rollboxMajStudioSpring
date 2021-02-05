using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChangeManager : MonoBehaviour
{

    public string nextLevel;


    public void NextLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }
}
