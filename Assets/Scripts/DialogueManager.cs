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



    //slider
    public Slider loveMeterSlider;
    public Image loveMeterFillImage;
    public Color lowLoveColor = Color.yellow;
    public Color highLoveColor = new Color(1f, 0.4f, 0.7f);


    //sounds
    public AudioSource audioSource;
    public AudioClip badSound;
    public AudioClip goodSound;
    public AudioClip neutralSound;



    //intermission UI
    public GameObject intermissionPanel;
    public TMP_Text scoreFeedbackText;
    public Button nextDateButton;
    public Button quitButton;



    //phone ui
    public Button phoneButton;
    public GameObject phoneMessagePanel;
    public Image phoneImage;
    public TMP_Text phoneMessageText;
    public Sprite bookImage;
    public Sprite catImage;



    //phone sound
    public AudioClip vibrateSound;
    public AudioSource sfxSource;

    //animation for phone
    private Coroutine vibrateRoutine;




    void Start()
    {

        totalScore = PlayerPrefs.GetInt("Score", 0);

        //set inactive
        phoneButton.gameObject.SetActive(false);
        phoneMessagePanel.SetActive(false);


        if (currentDialogue == null)
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
        }

        //phone listener
        phoneButton.onClick.AddListener(OpenPhoneMessage);


        ShowDialogueLine();
    }


    //to reset playerprefs and reset game
    public void ResetGame()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Date1");
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



    void UpdateLoveMeter()
    {
        loveMeterSlider.value = totalScore;
        float normalizedScore = loveMeterSlider.value / loveMeterSlider.maxValue;
        loveMeterFillImage.color = Color.Lerp(lowLoveColor, highLoveColor, normalizedScore);
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




    //shake sprite coroutine
    IEnumerator ShakeSprite(float duration = 0.3f, float magnitude = 0.2f)
    {
        Vector3 originalPos = girlRenderer.transform.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            girlRenderer.transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        girlRenderer.transform.localPosition = originalPos;
    }



    //good animation for sprite
    IEnumerator BounceSprite()
    {
        Vector3 originalScale = girlRenderer.transform.localScale;
        Vector3 targetScale = originalScale * 1.1f;

        float time = 0f;
        while (time < 0.15f)
        {
            girlRenderer.transform.localScale = Vector3.Lerp(originalScale, targetScale, time / 0.15f);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;
        while (time < 0.15f)
        {
            girlRenderer.transform.localScale = Vector3.Lerp(targetScale, originalScale, time / 0.15f);
            time += Time.deltaTime;
            yield return null;
        }

        girlRenderer.transform.localScale = originalScale;

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

        UpdateLoveMeter();

        // Change her facial expression
        switch (points)
        {
            case 0:
                girlRenderer.sprite = madSprite;
                audioSource.PlayOneShot(badSound);
                StartCoroutine(ShakeSprite());
                break;
            case 1:
                girlRenderer.sprite = neutralSprite;
                audioSource.PlayOneShot(neutralSound);

                break;
            case 2:
                girlRenderer.sprite = smileSprite;
                audioSource.PlayOneShot(goodSound);
                StartCoroutine(BounceSprite());
                break;
            default:
                girlRenderer.sprite = neutralSprite;
                break;
        }


        //check for message 1;
        if (PlayerPrefs.GetInt("Date", 1) == 1 && dialogueIndex == 3 && totalScore == 8)
        {
            TriggerPhone("Hes so cute isn't he!!!", catImage);
        }


        if (PlayerPrefs.GetInt("Date", 1) == 2 && dialogueIndex == 3 && totalScore == 14)
        {
            TriggerPhone("Heres the book I've been reading!", bookImage);
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
            ShowIntermissionScreen();
        }
    }





    void ShowIntermissionScreen()
    {


        intermissionPanel.SetActive(true);

        int currentDate = PlayerPrefs.GetInt("Date", 1);
        PlayerPrefs.SetInt("Date", currentDate +  1);
        PlayerPrefs.SetInt("Score", totalScore);


        //score feedback
        if (totalScore <= 8)
        {
            scoreFeedbackText.text = "Little Interest";

        } else if (totalScore <= 16) {
            scoreFeedbackText.text = "Somewhat Interested";

        } else {
            scoreFeedbackText.text = "Very interested";

        }


        //buttons
        nextDateButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();


        nextDateButton.onClick.AddListener(() =>
        {
            string nextScene = $"Date{PlayerPrefs.GetInt("Date", 1)}";
            SceneManager.LoadScene(nextScene);
        });

        quitButton.onClick.AddListener(() => Application.Quit());

    }




    void TriggerPhone(string message, Sprite image)
    {
        //activate phone
        phoneMessageText.text = message;
        phoneImage.sprite = image;

        phoneButton.gameObject.SetActive(true);


        //play sound
        if(sfxSource && vibrateSound)
        {
            sfxSource.PlayOneShot(vibrateSound);

        }

        //vibrate animation
        if (vibrateRoutine != null)
        {
            StopCoroutine(vibrateRoutine);

        }


        vibrateRoutine = StartCoroutine(VibratePhone());
    }




    IEnumerator VibratePhone(float duration = 1f, float magnitude = 10f)
    {
        RectTransform rect = phoneButton.GetComponent<RectTransform>();
        Vector3 originalPos = rect.anchoredPosition;

        float elapsed = 0f;
        while(elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            rect.anchoredPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rect.anchoredPosition = originalPos;
    }


    void OpenPhoneMessage()
    {
        phoneMessagePanel.SetActive(true);
        phoneButton.gameObject.SetActive(false);
    }


    public void ClosePhoneMessage()
    {
        phoneMessagePanel.SetActive(false);
    }


}
