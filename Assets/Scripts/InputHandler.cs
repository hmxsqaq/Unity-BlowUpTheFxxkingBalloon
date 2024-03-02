using System;
using Hmxs.Toolkit.Base.Singleton;
using UnityEngine.InputSystem;

public class InputHandler : SingletonMono<InputHandler>
{
    public InputControls InputControls;

    public InputAction act;
    public InputAction interact;
    public bool IsActing => act.phase == InputActionPhase.Performed;

    protected override void Awake()
    {
        base.Awake();
        InputControls = new InputControls();
        act = InputControls.Game.Act;
        interact = InputControls.UI.Interact;
        InputControls.Game.Enable();
        InputControls.UI.Disable();
    }

    private void Start()
    {
        // 继续游戏
        interact.performed += _ =>
        {
            InputControls.Game.Enable();
            InputControls.UI.Disable();
        };
    }

    private void OnDestroy() => InputControls.Disable();
}
