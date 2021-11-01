using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogControl : MonoBehaviour
{
    int clickTime;
    int showTime = 10;
    Consolation.LogUtils logPanel;
    private void Start()
    {
        clickTime = 0;
        GetComponent<Button>().onClick.AddListener(ShowLogPanel);
    }

    private void ShowLogPanel ()
    {
        clickTime++;
        if (clickTime > showTime)
        {
            try
            {
                logPanel = FindObjectOfType<Consolation.LogUtils>();
                Debug.unityLogger.logEnabled = true;
                logPanel.Show(true);
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
            clickTime = 0;
        }
    }
}
