using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;

public class LoadingScene : MonoBehaviour
{
    public Slider LoadingBar;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadLevelAsync());
    }

    // the async loader loaded the scene too fast and lag from start up was still visible. 
    // yes that means i did build in lag to the game
    // these extra two seconds should allow the game to load before the player can get in
    IEnumerator LoadLevelAsync()
    {
        float startTime = Time.realtimeSinceStartup;

        yield return new WaitForSecondsRealtime(0.5f);

        AsyncOperation operation = SceneManager.LoadSceneAsync("Subway");

        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f || Time.realtimeSinceStartup - startTime < 2f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            LoadingBar.value = progress;

            Debug.Log("progress is: " + operation.progress);
            
            yield return null;
        }
        LoadingBar.value = 1;
        operation.allowSceneActivation = true;
    }

    
}
