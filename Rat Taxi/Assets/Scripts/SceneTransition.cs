using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;

    public int score;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
}
