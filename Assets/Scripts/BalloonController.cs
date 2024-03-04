using Hmxs.Toolkit.Flow.Timer;
using Hmxs.Toolkit.Module.Events;
using Lofelt.NiceVibrations;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

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
    [SerializeField] private MMF_Player explosionFeedback;

    [Title("Info")]
    [ShowInInspector] [ReadOnly] private float _maxSize;
    [ShowInInspector] [ReadOnly] private float _sizeThreshold;
    [ShowInInspector] [ReadOnly] private float _currentSize;
    [ShowInInspector] [ReadOnly] private float _blowUpTimeCounter;
    [ShowInInspector] [ReadOnly] private float _blowUpSpeed;

    private void Start()
    {
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
            BlowUp();
        else
            _blowUpTimeCounter = 0;

        //if (InputHandler.Instance.act.WasPerformedThisFrame()) blowUpFeedbackInstant.PlayFeedbacks();
        if (InputHandler.Instance.act.WasReleasedThisFrame()) blowUpFeedbackInstant.PlayFeedbacks();
    }

    private void GameSetUp()
    {
        _maxSize = Random.Range(maxSizeRange.x, maxSizeRange.y);
        _sizeThreshold = Random.Range(sizeThresholdRange.x, sizeThresholdRange.y);
        _currentSize = initSize;
        transform.localScale = new Vector3(_currentSize, _currentSize, 1);
    }

    private void BlowUp()
    {
        _blowUpTimeCounter += Time.deltaTime;
        _blowUpSpeed = blowUpCurve.Evaluate(_blowUpTimeCounter);
        _currentSize = Mathf.Lerp(_currentSize, _maxSize, _blowUpSpeed * Time.deltaTime);
        transform.localScale = new Vector3(_currentSize, _currentSize, 1);

        cameraShake.PlayFeedbacks();
        blowUpFeedbackContinuous.PlayFeedbacks();
        HapticPatterns.PlayEmphasis(_currentSize / _maxSize, _currentSize / _maxSize);

        if (_currentSize >= _maxSize - _sizeThreshold)
        {
            cameraShake.StopFeedbacks();
            blowUpFeedbackContinuous.StopFeedbacks();
            InputHandler.Instance.InputControls.Game.Disable();
            Time.timeScale = 0.2f;
            explosionFeedback.PlayFeedbacks();
            Timer.Register(
                duration: 2f,
                onComplete: () => Events.Trigger(EventGroups.OnBalloonExploded),
                useRealTime: true
                );
            Debug.Log("气球爆炸");
        }
    }
}
