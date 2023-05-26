using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UserBehaviour : MonoBehaviour
{
    [Header("Left Hand")]
    public InputAction LeftTrigger;
    public InputAction LeftGrip;
    public InputAction LeftOne;
    public InputAction LeftTwo;
    public Transform leftHand;
    [Header("Right Hand")]
    public InputAction RightTrigger;
    public InputAction RightGrip;
    public InputAction RightOne;
    public InputAction RightTwo;
    public Transform rightHand;

    [Header("States")]
    public bool isLeftTriggerPressed;
    public bool isLeftGripPressed;
    public bool isLeftOnePressed;
    public bool isLeftTwoPressed;
    public bool isRightTriggerPressed;
    public bool isRightGripPressed;
    public bool isRightOnePressed;
    public bool isRightTwoPressed;

    [HideInInspector]
    public UnityEvent<Transform> OnLeftTriggerPressed;
    [HideInInspector]
    public UnityEvent<Transform> OnLeftGripPressed;
    [HideInInspector]
    public UnityEvent<Transform> OnLeftOnePressed;
    [HideInInspector]
    public UnityEvent<Transform> OnLeftTwoPressed;
    [HideInInspector]
    public UnityEvent<Transform> OnLeftTriggerRelease;
    [HideInInspector]
    public UnityEvent<Transform> OnLeftGripRelease;
    [HideInInspector]
    public UnityEvent<Transform> OnLeftOneRelease;
    [HideInInspector]
    public UnityEvent<Transform> OnLeftTwoRelease;

    [HideInInspector]
    public UnityEvent<Transform> OnRightTriggerPressed;
    [HideInInspector]
    public UnityEvent<Transform> OnRightGripPressed;
    [HideInInspector]
    public UnityEvent<Transform> OnRightOnePressed;
    [HideInInspector]
    public UnityEvent<Transform> OnRightTwoPressed;
    [HideInInspector]
    public UnityEvent<Transform> OnRightTriggerRelease;
    [HideInInspector]
    public UnityEvent<Transform> OnRightGripRelease;
    [HideInInspector]
    public UnityEvent<Transform> OnRightOneRelease;
    [HideInInspector]
    public UnityEvent<Transform> OnRightTwoRelease;


    public void OnEnable()
    {
        LeftTrigger.Enable();
        LeftGrip.Enable();
        LeftOne.Enable();
        LeftTwo.Enable();
        RightOne.Enable();
        RightTwo.Enable();
        RightTrigger.Enable();
        RightGrip.Enable();
    
        LeftTrigger.performed += ctx => { isLeftTriggerPressed = true; OnLeftTriggerPressed.Invoke(leftHand); };
        LeftTrigger.canceled += ctx => { isLeftTriggerPressed = false; OnLeftTriggerRelease.Invoke(leftHand); };
        LeftGrip.performed += ctx => { isLeftGripPressed = true; OnLeftGripPressed.Invoke(leftHand); };
        LeftGrip.canceled += ctx => { isLeftGripPressed = false; OnLeftGripRelease.Invoke(leftHand); };
        LeftOne.performed += ctx => { isLeftOnePressed = true; OnLeftOnePressed.Invoke(leftHand); };
        LeftOne.canceled += ctx => { isLeftOnePressed = false; OnLeftOneRelease.Invoke(leftHand); };
        LeftTwo.performed += ctx => { isLeftTwoPressed = true; OnLeftTwoPressed.Invoke(leftHand); };
        LeftTwo.canceled += ctx => { isLeftTwoPressed = false; OnLeftTwoRelease.Invoke(leftHand); };
        
        RightTrigger.performed += ctx => { isRightTriggerPressed = true; OnRightTriggerPressed.Invoke(rightHand); };
        RightTrigger.canceled += ctx => { isRightTriggerPressed = false; OnRightTriggerRelease.Invoke(rightHand); };
        RightGrip.performed += ctx => { isRightGripPressed = true; OnRightGripPressed.Invoke(rightHand); };
        RightGrip.canceled += ctx => { isRightGripPressed = false; OnRightGripRelease.Invoke(rightHand); };
        RightOne.performed += ctx => { isRightOnePressed = true; OnRightOnePressed.Invoke(rightHand); };
        RightOne.canceled += ctx => { isRightOnePressed = false; OnRightOneRelease.Invoke(rightHand); };
        RightTwo.performed += ctx => { isRightTwoPressed = true; OnRightTwoPressed.Invoke(rightHand); };
        RightTwo.canceled += ctx => { isRightTwoPressed = false; OnRightTwoRelease.Invoke(rightHand); };
    }

    
    public void OnDisable()
    {
        LeftTrigger.Disable();
        LeftGrip.Disable();
        LeftOne.Disable();
        LeftTwo.Disable();
        RightOne.Disable();
        RightTwo.Disable();
        RightTrigger.Disable();
        RightGrip.Disable();
    }
}
