using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private List<Sound> sounds;

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
            return;
        }
        AddAudioSource();
    }

    private void Start()
    {
        PlaySound("byebyebye",true);
    }

    private void AddAudioSource()
    {
        foreach (Sound item in sounds)
        {
            item.audioSource = gameObject.AddComponent<AudioSource>();
            item.audioSource.clip = item.clip;
            item.audioSource.volume = item.volume;
            item.audioSource.pitch = item.pitch;
            item.audioSource.playOnAwake = false;
        }
    }

    public void PlaySound(string soundName,bool loop)
    {
        Sound s = sounds.Find(sound => sound.name == soundName);
        s.audioSource.Play();
        s.audioSource.loop = loop;
    }

    public void PlayRandomScene()
    {
        int randomSceneIndex = Random.Range(1, SceneManager.sceneCountInBuildSettings - 1); // Sửa chỉ số giới hạn
        SceneManager.LoadScene(randomSceneIndex);
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }

    public void SetVolume(float volume)
    {
        foreach (Sound item in sounds)
        {
            item.audioSource.volume = volume;
        }
    }

    public void OnVolumeSliderChange(Slider slider)
    {
        SetVolume(slider.value);
    }
}
