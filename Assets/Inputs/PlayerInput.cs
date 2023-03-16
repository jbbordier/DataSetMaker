using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public XRIDefaultInputActions actions;
    Recorder recorder;
    private void Start()
    {
        actions = new XRIDefaultInputActions();
        recorder = transform.GetComponent<Recorder>();
        actions.XRILeftHandInteraction.Enable();
        actions.XRIRightHandInteraction.Enable();
        actions.XRILeftHandInteraction.Activate.performed += Recording;
        actions.XRILeftHandInteraction.Activate.canceled += StopRecording;
    }

    private void Recording(InputAction.CallbackContext obj)
    {
        recorder.Recording = true;
    }

    private void StopRecording(InputAction.CallbackContext obj)
    {
        recorder.Recording = false;
    }
}
