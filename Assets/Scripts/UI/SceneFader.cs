using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFader : MonoBehaviour
{
    CanvasGroup canvasGroup; // 其中的组件alpha用于控制画布透明度
    public float fadeInDuration; // 淡入持续时间
    public float fadeOutDuration; // 淡出持续时间

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        DontDestroyOnLoad(gameObject); // 确保场景切换时不会销毁此对象
    }

    public IEnumerator FadeOut(float time)
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / time; // 增加透明度
            yield return null;
        }
    }
    public IEnumerator FadeIn(float time)
    {
        while (canvasGroup.alpha >= 0)
        {
            canvasGroup.alpha -= Time.deltaTime / time; // 减少透明度
            yield return null;
        }

        Destroy(gameObject); // 淡入完成后销毁此对象
    }

}
