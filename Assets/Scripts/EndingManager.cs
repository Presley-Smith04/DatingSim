using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class EndingManager : MonoBehaviour
{
    //background
    public Image endingImage;
    public AudioSource audioSource;


    //sprites
    public Sprite ghostedSprite;
    public Sprite neutralSprite;
    public Sprite goodSprite;


    //audio
    public AudioClip ghostedMusic;
    public AudioClip neutralMusic;
    public AudioClip goodMusic;


    //zoom effect
    public float zoomDuration = 5f;
    public Vector3 startScale = new Vector3(1.2f, 1.2f, 1f);
    public Vector3 endScale = Vector3.one;


    //text
    public TextMeshProUGUI ghostedText;
    public TextMeshProUGUI neutralText;
    public TextMeshProUGUI goodText;




    void Start()
    {
        int score = PlayerPrefs.GetInt("Score", 0);
        SetupEnding(score);
    }


    
    //ending func
    void SetupEnding(int score)
    {
        //invisible text
        ghostedText.gameObject.SetActive(false);
        neutralText.gameObject.SetActive(false);
        goodText.gameObject.SetActive(false);


        endingImage.rectTransform.localScale = startScale;


        if (score <= 12)
        {
            endingImage.sprite = ghostedSprite;
            audioSource.clip = ghostedMusic;
            ghostedText.gameObject.SetActive(true);
            StartCoroutine(FadeIn(ghostedText));
        }
        else if (score <= 19)
        {
            endingImage.sprite = neutralSprite;
            audioSource.clip = neutralMusic;
            neutralText.gameObject.SetActive(true);
            StartCoroutine(FadeIn(neutralText));
        } else
        {
            endingImage.sprite = goodSprite;
            audioSource.clip = goodMusic;
            goodText.gameObject.SetActive(true);
            StartCoroutine(FadeIn(goodText));
        }

        audioSource.Play();
        StartCoroutine(ZoomOut());
    }





    //fade coroutine
    IEnumerator FadeIn(TextMeshProUGUI text, float duration = 4f)
    {
        text.alpha = 0;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            text.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
    }





    //zoom enumerator
    IEnumerator ZoomOut()
    {
        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            endingImage.rectTransform.localScale = Vector3.Lerp(startScale, endScale, elapsed / zoomDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        endingImage.rectTransform.localScale = endScale;
    } 


    public void RestartGame()
    {
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("Date", 1);
        SceneManager.LoadScene("StartMenu");
    }


    public void QuitGame()
    {
        Application.Quit();



        //used to stop play mode in the editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
