using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;
    public AudioSource audioSource;
    public AudioClip startSound;


    void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }


    void OnStartClicked()
    {
        StartCoroutine(StartDelay());
    }


    IEnumerator StartDelay()
    {
        if(audioSource != null && startSound != null)
        {
            audioSource.PlayOneShot(startSound);

            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("Date1");
        } 
    }


    void OnQuitClicked()
    {
        Application.Quit();
        

        //used to stop play mode in the editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; 
#endif
    }
}
