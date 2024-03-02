using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class ImageFadePlayableBehaviour : PlayableBehaviour
{
    public Image FadeImage;
    public AnimationCurve FadeCurve;
    public Color StartColor;
    public bool CloseOnEnd;

    public FadeType FadeType;
    public float TargetAlpha;
    public Color TargetColor;

    private Color _color;
    private float _startAlpha;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (FadeImage == null) return;
        FadeImage.color = StartColor;
        _color = StartColor;
        _startAlpha = StartColor.a;
        FadeImage.gameObject.SetActive(true);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (FadeImage == null) return;
        var time = playable.GetTime();
        var duration = playable.GetDuration();
        switch (FadeType)
        {
            case FadeType.Alpha:
                var alpha = Mathf.Lerp(_startAlpha, TargetAlpha, FadeCurve.Evaluate((float)(time / duration)));
                _color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, alpha);
                break;
            case FadeType.Color:
                _color = Color.Lerp(StartColor, TargetColor, FadeCurve.Evaluate((float)(time / duration)));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        FadeImage.color = _color;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (FadeImage == null) return;
        FadeImage.color = FadeType switch
        {
            FadeType.Alpha => new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, TargetAlpha),
            FadeType.Color => TargetColor,
            _ => throw new ArgumentOutOfRangeException()
        };
        if (CloseOnEnd) FadeImage.gameObject.SetActive(false);
    }
}
