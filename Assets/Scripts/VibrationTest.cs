using UnityEngine;
using Lofelt.NiceVibrations;

public class VibrationTest : MonoBehaviour
{
    public float amplitude = 1.0f;
    public float frequency = 1.0f;
    public HapticPatterns.PresetType presetType;
    public HapticClip hapticClip;

    public void Vibrate()
    {
        Debug.Log("Vibrating!");
        //HapticPatterns.PlayEmphasis(amplitude, frequency);
        // HapticPatterns.PlayPreset(presetType);
        HapticController.Play(hapticClip);
    }
}
