using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    public XRIDefaultInputActions actions;
    Recorder recorder;
    GameManagerDemo gameManagerDemo;
    private void Start()
    {
        actions = new XRIDefaultInputActions();
        recorder = transform.GetComponent<Recorder>();
        gameManagerDemo = transform.GetComponent<GameManagerDemo>();
        actions.XRILeftHandInteraction.Enable();
        actions.XRIRightHandInteraction.Enable();
        actions.XRILeftHandInteraction.Activate.performed += Recording;
        actions.XRILeftHandInteraction.Activate.canceled += StopRecording;
        actions.XRIRightHandInteraction.Activate.performed += LoadAnim;
    }

    private void Recording(InputAction.CallbackContext obj)
    {
        if (SceneManager.GetActiveScene().name.Contains("Demo"))
        {
            gameManagerDemo.Recording = true;
        }
        else
        {
            recorder.Recording = true;
        }
    }

    private void StopRecording(InputAction.CallbackContext obj)
    {
        if (SceneManager.GetActiveScene().name.Contains("Demo"))
        {
            gameManagerDemo.Recording = false;
        }
        else
        {
            recorder.Recording = false;
        }
    }

    private void LoadAnim(InputAction.CallbackContext obj)
    {
        if (SceneManager.GetActiveScene().name.Contains("Demo"))
        {
            gameManagerDemo.SimulateAnimation();
        }

    }
}
