using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public enum FadeType
{
    Alpha,
    Color
}

[System.Serializable]
public class ImageFadePlayableAsset : PlayableAsset
{
    [SerializeField] private ExposedReference<Image> fadeImage;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private Color startColor = Color.black;
    [SerializeField] private bool closeOnEnd;

    [EnumToggleButtons]
    [SerializeField] private FadeType fadeType = FadeType.Alpha;

    [ShowIf("fadeType", FadeType.Alpha)]
    [SerializeField] private float targetAlpha;
    [ShowIf("fadeType", FadeType.Color)]
    [SerializeField] private Color targetColor = Color.black;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ImageFadePlayableBehaviour>.Create(graph);
        var fadeBehaviour = playable.GetBehaviour();
        fadeBehaviour.FadeImage = fadeImage.Resolve(graph.GetResolver());
        fadeBehaviour.FadeCurve = fadeCurve;
        fadeBehaviour.StartColor = startColor;
        fadeBehaviour.CloseOnEnd = closeOnEnd;
        fadeBehaviour.FadeType = fadeType;
        fadeBehaviour.TargetAlpha = targetAlpha;
        fadeBehaviour.TargetColor = targetColor;
        return playable;
    }
}