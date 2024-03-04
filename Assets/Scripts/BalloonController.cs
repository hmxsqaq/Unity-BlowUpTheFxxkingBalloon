using System.Collections.Generic;
using Hmxs.Toolkit.Flow.Timer;
using Hmxs.Toolkit.Module.Audios;
using Hmxs.Toolkit.Module.Events;
using Lofelt.NiceVibrations;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;
using AudioType = Hmxs.Toolkit.Module.Audios.AudioType;

public class BalloonController : MonoBehaviour
{
    [Title("Settings")]
    [LabelText("气球初始大小")]
    [SerializeField] private float initSize;

    [LabelText("气球最大大小范围")]
    [MinMaxSlider(0.5f, 5f, true)]
    [SerializeField] private Vector2 maxSizeRange = new (1f, 3f);

    [LabelText("气球大小增长曲线")]
    [SerializeField] private AnimationCurve blowUpCurve;

    [LabelText("气球爆炸阈值修正范围(越小越不容易爆)")]
    [MinMaxSlider(0.001f, 0.8f, true)]
    [SerializeField] private Vector2 sizeThresholdRange = new (0.005f, 0.5f);

    [Title("Feedbacks")]
    [SerializeField] private MMF_Player cameraShake;
    [SerializeField] private MMF_Player blowUpFeedbackInstant;
    [SerializeField] private MMF_Player blowUpFeedbackContinuous;
    [SerializeField] private MMF_Player blowUpEndFeedback;
    [SerializeField] private MMF_Player explosionFeedback;
    [SerializeField] private MMF_Player audioFeedback;
    [SerializeField] private MMF_Player blowUpAudioFeedback;
    [SerializeField] private MMF_Player boomAudioFeedback;
    [SerializeField] private GameObject boomEffect;

    [Title("Info")]
    [SerializeField] [ReadOnly] private float maxSize;
    [SerializeField] [ReadOnly] private float sizeThreshold;
    [SerializeField] [ReadOnly] private float currentSize;
    [SerializeField] [ReadOnly] private float blowUpTimeCounter;
    [SerializeField] [ReadOnly] private float blowUpSpeed;

    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        AudioCenter.Instance.AudioPlaySync(new AudioAsset(AudioType.BGM, "背景音", true));
        AudioCenter.Instance.SetVolume(AudioType.BGM, 0.2f);

        GameSetUp();
        // 继续游戏
        InputHandler.Instance.interact.performed += _ =>
        {
            Time.timeScale = 1;
            GameSetUp();
        };
    }

    private void Update()
    {
        if (InputHandler.Instance.IsActing)
        {
            BlowUp();
            if (blowUpFeedbackContinuous.isActiveAndEnabled)
                blowUpFeedbackContinuous.StopFeedbacks();
        }
        else
        {
            if (currentSize / (maxSize - sizeThreshold) > 0.75 && !blowUpFeedbackContinuous.isActiveAndEnabled)
                blowUpFeedbackContinuous.PlayFeedbacks();

            if (blowUpFeedbackInstant.isActiveAndEnabled)
                blowUpFeedbackInstant.StopFeedbacks();

            if (blowUpAudioFeedback.isActiveAndEnabled)
                blowUpAudioFeedback.StopFeedbacks();

            blowUpTimeCounter = 0;
        }

        // if (InputHandler.Instance.act.WasPerformedThisFrame())
        //     audioFeedback.PlayFeedbacks();

        if (InputHandler.Instance.act.WasReleasedThisFrame())
        {
            blowUpEndFeedback.PlayFeedbacks();
            audioFeedback.PlayFeedbacks();
        }
    }

    private void GameSetUp()
    {
        _spriteRenderer.enabled = true;
        maxSize = Random.Range(maxSizeRange.x, maxSizeRange.y);
        sizeThreshold = Random.Range(sizeThresholdRange.x, sizeThresholdRange.y);
        currentSize = initSize;
        transform.localScale = new Vector3(currentSize, currentSize, 1);
    }

    private void BlowUp()
    {
        blowUpTimeCounter += Time.deltaTime;
        blowUpSpeed = blowUpCurve.Evaluate(blowUpTimeCounter);
        currentSize = Mathf.Lerp(currentSize, maxSize, blowUpSpeed * Time.deltaTime);
        transform.localScale = new Vector3(currentSize, currentSize, 1);

        cameraShake.PlayFeedbacks();
        blowUpFeedbackInstant.PlayFeedbacks();
        HapticPatterns.PlayEmphasis(currentSize / maxSize, currentSize / maxSize);
        if (!blowUpAudioFeedback.isActiveAndEnabled) blowUpAudioFeedback.PlayFeedbacks(); // 吹气音效

        if (currentSize >= maxSize - sizeThreshold)
        {
            if (cameraShake.isActiveAndEnabled)
                cameraShake.StopFeedbacks();
            if (blowUpFeedbackInstant.isActiveAndEnabled)
                blowUpFeedbackInstant.StopFeedbacks();
            InputHandler.Instance.InputControls.Game.Disable(); // 禁用输入
            HapticPatterns.PlayConstant(1, 1, 1); // 振动
            Instantiate(boomEffect); // 爆炸动画
            explosionFeedback.PlayFeedbacks(); // 爆炸粒子
            boomAudioFeedback.PlayFeedbacks(); // 播放爆炸音效
            _spriteRenderer.enabled = false; // 隐藏气球

            Time.timeScale = 0.2f;

            Timer.Register(
                duration: 2f,
                onComplete: () => Events.Trigger(EventGroups.OnBalloonExploded),
                useRealTime: true
                );
        }
    }
}
