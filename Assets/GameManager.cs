using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject loadScene;
    public Slider loadSlider;
    public Transform fan;
    void Start()
    {
        loadScene.SetActive(true);
        loadSlider.value = 0;
    }

    
    void Update()
    {
        loadScreen();
        Fan();
    }

    private void loadScreen()
    {
        loadSlider.value += Random.Range(0.0005f, 0.015f);
        if (loadSlider.value >=1)
        {
            loadScene.SetActive(false);
        }
    }
    private void Fan()
    {
        fan.Rotate(Vector3.up,120f * Time.deltaTime);
    }
}
