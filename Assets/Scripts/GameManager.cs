using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Scene Loading and Exit")]
    public GameObject loadScene;
    public Slider loadSlider;
    public GameObject ExitTable;
    private bool exit;
    public GameObject GameOverTable;

    public List<Transform> fans;

    [Header("Time and Score")]
    public List<TextMeshProUGUI> scores;
    public TextMeshProUGUI timerText;
    private int numberScore;
    [Networked] private int minutes { get; set; }
    [Networked] private int seconds { get; set; }

    [Header("Chart")]
    public GameObject chart;
    private bool charted;

    [Header("Volume")]
    public Slider volume;

    private Dictionary<NetworkId, int> playerScores = new Dictionary<NetworkId, int>();

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
        SetVolume();
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

    public void UpdateScoreForPlayer(NetworkId playerId, int score)
    {
        // Cập nhật hoặc thêm điểm số cho player
        if (playerScores.ContainsKey(playerId))
        {
            playerScores[playerId] = score;
        }
        else
        {
            playerScores.Add(playerId, score);
        }

        // Cập nhật hiển thị điểm số
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        // Sắp xếp điểm số theo thứ tự giảm dần
        var sortedScores = playerScores.OrderByDescending(entry => entry.Value).ToList();

        // Cập nhật điểm số vào UI
        for (int i = 0; i < scores.Count; i++)
        {
            if (i < sortedScores.Count)
            {
                var playerScore = sortedScores[i];
                scores[i].text = "Player " + playerScore.Key + ": " + playerScore.Value;
            }
            else
            {
                // Xóa hiển thị nếu không đủ điểm số để hiển thị
                scores[i].text = "";
            }
        }
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
        if (!charted)
        {
            exit = !exit;
            ExitTable.SetActive(exit);
        }
    }

    public void EnterExit()
    {
        SceneManager.LoadScene(0);
    }

    public void EnterCencel()
    {
        ExitTable.SetActive(false);
        exit = false;
    }

    public void Chart()
    {
        if (!exit)
        {
            charted = !charted;
            chart.SetActive(charted);
        }
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
            if (minutes == 0 && seconds <= 30)
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
        timerText.text = "00:00";
        ShowChart();
    }

    public void ShowChart()
    {
        chart.SetActive(true);
        charted = true;
    }

    public void GameOver()
    {
        ShowChart();
        GameOverTable.SetActive(true);
    }

    public void SetVolume()
    {
        SoundManager.instance.OnVolumeSliderChange(volume);
    }
}
