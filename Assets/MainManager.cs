using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    public static MainManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Play()
    {
        int randomSceneIndex = Random.Range(1, 2);
        SceneManager.LoadScene(randomSceneIndex);
    }


    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
}
