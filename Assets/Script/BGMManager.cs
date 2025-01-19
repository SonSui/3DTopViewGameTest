using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


[System.Serializable]
public struct SceneAudio
{
    public string sceneName;      
    public AudioClip audioClip;
    public float volume;
}


[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    [Header("BGM")]
    public List<SceneAudio> sceneAudioList = new List<SceneAudio>();

    private AudioSource audioSource;
    private Dictionary<string, SceneAudio> sceneToClip = new Dictionary<string, SceneAudio>();

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
        audioSource.loop = true;

        foreach (var item in sceneAudioList)
        {
            if (!sceneToClip.ContainsKey(item.sceneName))
            {
                sceneToClip.Add(item.sceneName, item);
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
        if (sceneToClip.TryGetValue(scene.name, out SceneAudio targetAudio))
        {
            
            if (audioSource.clip == targetAudio.audioClip && audioSource.isPlaying)
            {
                return;
            }

            
            audioSource.Stop();
            audioSource.clip = targetAudio.audioClip;
            audioSource.volume = targetAudio.volume;
            audioSource.Play();
            
        }
       
    }
}
