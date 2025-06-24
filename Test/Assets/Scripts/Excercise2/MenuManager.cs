using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DeviceInfoPluginTools;

public class MenuManager : MonoBehaviour
{


    [SerializeField]
    private TMP_Text Console = null;
    [SerializeField]
    private Button button = null;

    private void Awake()
    {
        button.onClick.AddListener(Execute);
    }

    private void Execute()
    {
        string message = $"{DeviceInfoPlugin.GetCurrentDateTimeString()}\n{DeviceInfoPlugin.GetCurrentLocaleString()}";
        if (string.IsNullOrWhiteSpace(message))
            message = "DLL didn't brougt nothing.";

        Console.text = message;
    }
}
