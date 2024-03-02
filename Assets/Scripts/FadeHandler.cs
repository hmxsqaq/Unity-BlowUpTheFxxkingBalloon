using System;
using Hmxs.Toolkit.Flow.Timer;
using UnityEngine;
using UnityEngine.UI;

public static class FadeHandler
{
    public static void Fade(Graphic graphic, float duration, float startAlpha, float targetAlpha, AnimationCurve curve = null, bool useRealTime = false, Action onComplete = null)
    {
        if (graphic == null) return;
        curve ??= AnimationCurve.Linear(0f, 0f, 1f, 1f);
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, startAlpha);
        graphic.gameObject.SetActive(true);
        Timer.Register(
            duration: duration,
            onComplete: onComplete,
            onUpdate: time =>
            {
                var alpha = Mathf.Lerp(startAlpha, targetAlpha, curve.Evaluate(time / duration));
                var color = graphic.color;
                color = new Color(color.r, color.g, color.b, alpha);
                graphic.color = color;
            },
            useRealTime: useRealTime
            );
    }

    public static void Fade(Graphic graphic, float duration, Color startColor, Color targetColor, AnimationCurve curve = null, bool useRealTime = false, Action onComplete = null)
    {
        if (graphic == null) return;
        curve ??= AnimationCurve.Linear(0f, 0f, 1f, 1f);
        graphic.color = startColor;
        graphic.gameObject.SetActive(true);
        Timer.Register(
            duration: duration,
            onComplete: onComplete,
            onUpdate: time =>
            {
                var color = Color.Lerp(startColor, targetColor, curve.Evaluate(time / duration));
                graphic.color = color;
            },
            useRealTime: useRealTime
            );
    }
}
