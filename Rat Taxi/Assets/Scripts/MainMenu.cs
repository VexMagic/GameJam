using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    float timer;

    private void Update()
    {
        if (Input.anyKeyDown && timer >= 1)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}
