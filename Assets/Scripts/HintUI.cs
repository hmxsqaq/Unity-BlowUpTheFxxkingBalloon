using System;
using Hmxs.Toolkit.Module.Events;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintUI : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField] private Image hintPanel;
    [SerializeField] private float idleTimeLimit;

    [SerializeField] private Image nextPanel;
    [SerializeField] private TextMeshProUGUI nextText;

    [Title("Info")]
    [SerializeField] [ReadOnly] private float idleTimeCounter;

    private bool _isHintPanelActive;

    private void Start() =>
        // 继续游戏
        InputHandler.Instance.interact.performed += _ => nextPanel.gameObject.SetActive(false);

    private void OnEnable() => Events.AddListener(EventGroups.OnBalloonExploded, OpenNextPanel);
    private void OnDisable() => Events.RemoveListener(EventGroups.OnBalloonExploded, OpenNextPanel);

    private void OpenNextPanel()
    {
        FadeHandler.Fade(
            nextPanel, 1f, 0, 0.5f,
            useRealTime: true,
            onComplete: () => InputHandler.Instance.InputControls.UI.Enable()
        );
        FadeHandler.Fade(
            nextText, 1f, 0, 1f,
            useRealTime: true
        );
        hintPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!InputHandler.Instance.InputControls.Game.enabled) return;

        if (InputHandler.Instance.act.IsPressed())
        {
            idleTimeCounter = 0;
            _isHintPanelActive = false;
            hintPanel.gameObject.SetActive(false);
        }
        else if (!_isHintPanelActive)
        {
            if (idleTimeCounter > idleTimeLimit)
            {
                _isHintPanelActive = true;
                FadeHandler.Fade(
                    hintPanel, 0.5f, 0, 0.5f,
                    useRealTime: true
                );
            }
            else
                idleTimeCounter += Time.deltaTime;
        }
    }
}
