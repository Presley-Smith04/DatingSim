using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public DialogueData currentDialogue;
    public TMP_Text dialogueText;
    public Button[] choiceButtons;
    public TMP_Text[] choiceTexts;

    private int dialogueIndex = 0;
    private bool waitingForSpace = false;
    private int totalScore = 0;



    void Start()
    {
        // Get the current date number (defaults to 1)
        int dateNumber = PlayerPrefs.GetInt("Date", 1);

        // Build the dialogue asset name based on the date
        string name = $"Date{dateNumber}Dialogue";

        // Load the DialogueData asset from the Resources folder
        currentDialogue = Resources.Load<DialogueData>(name);

        if (currentDialogue == null)
        {
            Debug.LogError($"Dialogue file '{name}' not found in Resources.");
            return;
        }

        ShowDialogueLine();
    }



    void Update()
    {
        if (waitingForSpace && Input.GetKeyDown(KeyCode.Space))
        {
            waitingForSpace = false;
            EnableChoices(true);
        }
    }



    void ShowDialogueLine()
    {
        DialogueEntry entry = currentDialogue.dateDialogue[dialogueIndex];
        dialogueText.text = entry.girlDialogue;


        for (int i = 0; i < 4; i++)
        {
            choiceTexts[i].text = entry.playerChoices[i];
        }
        EnableChoices(false);
        waitingForSpace = true;
    }


    void EnableChoices(bool enabled)
    {
        foreach (Button btn in choiceButtons)
            btn.gameObject.SetActive(enabled);
    }



    public void ChooseOption(int index)
    {
        DialogueEntry entry = currentDialogue.dateDialogue[dialogueIndex];
        totalScore += entry.choicePoints[index];
        dialogueText.text = entry.girlResponses[index];
        dialogueIndex++;

        if (dialogueIndex < currentDialogue.dateDialogue.Length)
        {
            // Wait and show next line
            Invoke("ShowDialogueLine", 1.5f);
        }
        else
        {
            // Save score and increment date
            PlayerPrefs.SetInt("Score", totalScore);
            int currentDate = PlayerPrefs.GetInt("Date", 1);
            PlayerPrefs.SetInt("Date", currentDate + 1);

            // Load next scene
            if (currentDate < 3)
            {
                string nextScene = $"Date{currentDate + 1}";
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("EndScene");
            }
        }

        EnableChoices(true);
    }


}
