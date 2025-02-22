using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    //make screen dark after scene change
    public static FadeManager Instance { get; private set; }
    public Image fadeImage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void FadeIn(float duration = 2f)
    {
        StartCoroutine(FadeEffect(1, 0, duration));
    }

    public void FadeOut(float duration = 2f)
    {
        StartCoroutine(FadeEffect(0, 1, duration));
    }

    private IEnumerator FadeEffect(float start, float end, float duration)
    {
        //ˆÃ‚¢‚©‚ç–¾‚é‚­‚È‚é
        float time = 0;
        Color color = fadeImage.color;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(start, end, time / duration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, end);
    }
}