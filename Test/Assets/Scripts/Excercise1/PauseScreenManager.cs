using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PauseScreenManager : MonoBehaviour
{
    [SerializeField]
    private Slider jumpSlider = null;
    [SerializeField]
    private Button backButton = null;

    public Slider JumpSlider {
        get { return jumpSlider; }
    }
    
    private void Awake()
    {
        InputSystem.actions.FindAction("Cancel").performed += TogglePause;
        gameObject.SetActive(false);
    }

    public void ClosePauseScreen()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void TogglePause(InputAction.CallbackContext callbackContext)
    {
        if (gameObject.activeSelf)
        {
            ClosePauseScreen();
        }
        else
        {
            gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
