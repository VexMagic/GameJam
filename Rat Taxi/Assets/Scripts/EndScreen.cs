using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;

    void Start()
    {
        score.text = "Score: " + SceneTransition.instance.score;   
    }
}
