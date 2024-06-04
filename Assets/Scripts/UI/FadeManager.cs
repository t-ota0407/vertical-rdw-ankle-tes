using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    private const float defaultFadeDuration = 1;

    [SerializeField] private FadeStatus fadeStatus = FadeStatus.FADED_IN;
    [SerializeField] private Image fadeImage;

    public FadeStatus FadeStatus { get { return fadeStatus; } }

    void Start()
    {
        if (fadeStatus == FadeStatus.FADING_OUT || fadeStatus == FadeStatus.FADING_IN)
        {
            fadeStatus = FadeStatus.FADED_IN;
        }

        if (fadeStatus == FadeStatus.FADED_IN)
        {
            StartFadeIn(0.001f);
        }
        else if (fadeStatus == FadeStatus.FADED_OUT)
        {
            StartFadeOut(0.001f);
        }
    }

    public void StartFadeOut(float fadeDuration = defaultFadeDuration)
    {
        fadeStatus = FadeStatus.FADING_OUT;
        StartCoroutine(FadeOut(fadeDuration));
    }

    public void StartFadeIn(float fadeDuration = defaultFadeDuration)
    {
        fadeStatus = FadeStatus.FADING_IN;
        StartCoroutine(FadeIn(fadeDuration));
    }

    private IEnumerator FadeOut(float fadeDuration)
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float fadeImageAlpha = Mathf.Clamp(elapsedTime / fadeDuration, 0, 1);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeImageAlpha);
            yield return null;
        }

        fadeStatus = FadeStatus.FADED_OUT;
    }

    private IEnumerator FadeIn(float fadeDuration)
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float fadeImageAlpha = Mathf.Clamp(1.0f - (elapsedTime / fadeDuration), 0, 1);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, fadeImageAlpha);
            yield return null;
        }

        fadeStatus = FadeStatus.FADED_IN;
    }
}
