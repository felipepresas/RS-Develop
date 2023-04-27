using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class ToggleFullscreen : MonoBehaviour
{
    public UnityEvent<bool> OnFullscreenToggled;
    public UnityEvent<int> OnResolutionChanged;
    public UnityEvent<int> OnQualityLevelChanged;

    private Resolution[] resolutions;
    private int currentResolutionIndex;

    private void Start()
    {
        resolutions = Screen.resolutions;
        currentResolutionIndex = GetClosestResolutionIndex(Screen.currentResolution);

        // Invoca eventos al iniciar para establecer el estado inicial
        OnFullscreenToggled?.Invoke(Screen.fullScreen);
        OnResolutionChanged?.Invoke(currentResolutionIndex);
        OnQualityLevelChanged?.Invoke(QualitySettings.GetQualityLevel());
    }

    private void Update()
    {
        HandleFullscreenToggle();
        HandleResolutionChange();
        HandleQualityLevelChange();
    }

    private void HandleFullscreenToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F11) ||
            (Input.GetKeyDown(KeyCode.Return) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))))
        {
            ToggleFullscreenMode();
        }
    }

    private void HandleResolutionChange()
    {
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            IncrementResolution();
        }

        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            DecrementResolution();
        }
    }

    private void HandleQualityLevelChange()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus))
        {
            IncrementQualityLevel();
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
        {
            DecrementQualityLevel();
        }
    }

    private void ToggleFullscreenMode()
    {
        Screen.fullScreen = !Screen.fullScreen;
        OnFullscreenToggled?.Invoke(Screen.fullScreen);
    }

    private void IncrementResolution()
    {
        currentResolutionIndex = Mathf.Clamp(currentResolutionIndex + 1, 0, resolutions.Length - 1);
        SetResolution(currentResolutionIndex);
    }

    private void DecrementResolution()
    {
        currentResolutionIndex = Mathf.Clamp(currentResolutionIndex - 1, 0, resolutions.Length - 1);
        SetResolution(currentResolutionIndex);
    }

    private void SetResolution(int index)
    {
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);
        OnResolutionChanged?.Invoke(index);
    }

    private int GetClosestResolutionIndex(Resolution targetResolution)
    {
        int closestIndex = 0;
        float minDifference = float.MaxValue;

        for (int i = 0; i < resolutions.Length; i++)
        {
            float difference = Mathf.Abs(resolutions[i].width - targetResolution.width) +
                               Mathf.Abs(resolutions[i].height - targetResolution.height);

            if (difference < minDifference)
            {
                minDifference = difference;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    private void IncrementQualityLevel()
    {
        QualitySettings.IncreaseLevel();
        OnQualityLevelChanged?.Invoke(QualitySettings.GetQualityLevel());
    }

    private void DecrementQualityLevel()
    {
        QualitySettings.DecreaseLevel();
        OnQualityLevelChanged?.Invoke(QualitySettings.GetQualityLevel());
    }
}
