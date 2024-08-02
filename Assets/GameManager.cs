using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject loadScene;
    public Slider loadSlider;
    public List<Transform> fans;
    public TextMeshProUGUI score;
    public TextMeshProUGUI timerText; // Assuming you have a TextMeshProUGUI component to display the timer
    private int numberScore;
    private int minutes;
    private int seconds;
    private int ticks;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        loadScene.SetActive(true);
        loadSlider.value = 0;
        numberScore = 0;
        StartCoroutine(TimePlay());
    }


    void Update()
    {
        loadScreen();
        Fan();
    }

    private void loadScreen()
    {
        loadSlider.value += Random.Range(0.0005f, 0.015f);
        if (loadSlider.value >= 1)
        {
            loadScene.SetActive(false);
        }
    }

    private void Fan()
    {
        foreach (var item in fans)
        {
            item.Rotate(Vector3.up, 120f * Time.deltaTime);
        }
    }

    public void Score(int number)
    {
        numberScore += number;
        score.text = numberScore.ToString();
    }

    public void StartReActiveRUby(GameObject obj, float delay)
    {
        StartCoroutine(ReactivateAfterDelay(obj, delay));
    }

    private IEnumerator ReactivateAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(true);
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }

    private IEnumerator TimePlay()
    {
        int totalSeconds = 120;

        while (totalSeconds > 0)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            if (timerText != null)
            {
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            if(minutes == 0 && seconds <= 30)
            {
                timerText.color = Color.red;
            }
            yield return new WaitForSeconds(1f);
            totalSeconds--;
        }
        OnTimerEnd();
    }

    private void OnTimerEnd()
    {
        Debug.Log("Timer ended!");
    }
}
