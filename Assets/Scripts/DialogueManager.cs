using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;


public class DialogueManager : MonoBehaviour
{
    //dialogue and button data stuff
    public DialogueData currentDialogue;
    public TMP_Text dialogueText;
    public Button[] choiceButtons;
    public TMP_Text[] choiceTexts;


    //set data to 0, wait for input to continue
    private int dialogueIndex = 0;
    private bool waitingForSpace = false;
    private int totalScore = 0;
    private Coroutine typingCoroutine;


    //girl sprites
    public SpriteRenderer girlRenderer;
    public Sprite smileSprite;
    public Sprite neutralSprite;
    public Sprite madSprite;





    void Start()
    {
        //get date number
        int dateNumber = PlayerPrefs.GetInt("Date", 1);

        //build dialogue based on date #
        string name = $"Date{dateNumber}Dialogue";

        //load dialogue
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
        //wait for input
        if (waitingForSpace && Input.GetKeyDown(KeyCode.Space))
        {
            waitingForSpace = false;
            EnableChoices(true);
        }
    }




    void ShowDialogueLine()
    {
        //show dialogue
        DialogueEntry entry = currentDialogue.dateDialogue[dialogueIndex];


        //typewriter effect
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);


        typingCoroutine = StartCoroutine(TypeText(entry.girlDialogue));

        //player choices
        for (int i = 0; i < 4; i++)
        {
            choiceTexts[i].text = entry.playerChoices[i];
        }

        EnableChoices(false);
    }





    //typewriter text style
    IEnumerator TypeText(string line)
    {
        dialogueText.text = "";
        waitingForSpace = false;


        //typing sppeeed
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.03f);
        }

        waitingForSpace = true;
    }



    void EnableChoices(bool enabled)
    {
        //activate choices
        foreach (Button btn in choiceButtons)
            btn.gameObject.SetActive(enabled);
    }




    public void ChooseOption(int index)
    {
        DialogueEntry entry = currentDialogue.dateDialogue[dialogueIndex];

        int points = entry.choicePoints[index];
        totalScore += points;

        // Change her facial expression
        switch (points)
        {
            case 0:
                girlRenderer.sprite = madSprite;
                break;
            case 1:
                girlRenderer.sprite = neutralSprite;
                break;
            case 2:
                girlRenderer.sprite = smileSprite;
                break;
            default:
                girlRenderer.sprite = neutralSprite;
                break;
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // Start coroutine to show response and wait for space
        typingCoroutine = StartCoroutine(ShowResponseThenNext(entry.girlResponses[index]));

        EnableChoices(false);
    }



    IEnumerator ShowResponseThenNext(string response)
    {

        //girl response
        dialogueText.text = "";
        waitingForSpace = false;


        //typing speedd
        foreach(char c in response.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.03f);
        }

        waitingForSpace = true;



        //wait for it to be pressed
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        waitingForSpace = false;
        dialogueIndex++;


        //show dialogue if you havent
        if (dialogueIndex < currentDialogue.dateDialogue.Length)
        {
            ShowDialogueLine();
        } else
        {
            //set score and increase date
            PlayerPrefs.SetInt("Score", totalScore);
            int currentDate = PlayerPrefs.GetInt("Date", 1);
            PlayerPrefs.SetInt("Date", currentDate + 1);


            //next date
            if(currentDate < 3)
            {
                string nextScene = $"Date{currentDate + 1}";
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
            } else
            {
                //end 
                UnityEngine.SceneManagement.SceneManager.LoadScene("Ending");

            }
        }
    }


}
