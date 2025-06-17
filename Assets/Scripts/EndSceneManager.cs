using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndSceneManager : MonoBehaviour
{
    public TMP_Text resultText;

    void Start()
    {
        int score = PlayerPrefs.GetInt("Score", 0);
        int date = PlayerPrefs.GetInt("Date", 0);

        if (date < 3)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("DateScene");
            return;
        }

        if (score <= 19) resultText.text = "You got ghosted.";
        else if (score <= 25) resultText.text = "You're just friends.";
        else resultText.text = "Dating";
    }
}
