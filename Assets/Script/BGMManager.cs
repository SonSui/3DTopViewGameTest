using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


[System.Serializable]
public struct SceneAudio
{
    public string sceneName;      
    public AudioClip audioClip;   
}


[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    [Header("场景与对应的 BGM 配置列表")]
    public List<SceneAudio> sceneAudioList = new List<SceneAudio>();

    private AudioSource audioSource;
    private Dictionary<string, AudioClip> sceneToClip = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        
        audioSource = GetComponent<AudioSource>();

        
        foreach (var item in sceneAudioList)
        {
            if (!sceneToClip.ContainsKey(item.sceneName))
            {
                sceneToClip.Add(item.sceneName, item.audioClip);
            }
        }
    }

    private void OnEnable()
    {
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (sceneToClip.TryGetValue(scene.name, out AudioClip targetClip))
        {
            
            if (audioSource.clip == targetClip && audioSource.isPlaying)
            {
                // Do nothing
                return;
            }

            
            audioSource.Stop();
            audioSource.clip = targetClip;
            audioSource.Play();
        }
        else
        {
            
        }
    }
}
