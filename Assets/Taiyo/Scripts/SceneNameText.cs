using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class SceneNameText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<TMP_Text>().text = SceneManager.GetActiveScene().name;
    }


}
