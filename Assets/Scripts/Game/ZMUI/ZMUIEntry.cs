using System;
using System.Collections;
using System.Collections.Generic;
using Framework.ZMUI;
using Game.ZMUI;
using UnityEngine;

public class ZMUIEntry : MonoBehaviour
{
    private void Awake()
    {
        UIModule.Instance.Init();
    }
    
    void Start()
    {
        UIModule.Instance.PushWindowToStack<TestStackWindow1>();
        UIModule.Instance.PushWindowToStack<TestStackWindow2>();
        UIModule.Instance.PushWindowToStack<TestStackWindow3>();
    }
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            UIModule.Instance.PopUpWindow<TestWindow>();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            UIModule.Instance.PopUpWindow<MainWindow>();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UIModule.Instance.PushWindowToStack<TestStackWindow1>();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UIModule.Instance.PushWindowToStack<TestStackWindow2>();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UIModule.Instance.PushWindowToStack<TestStackWindow3>();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UIModule.Instance.PopUpWindow<TestStackWindow1>();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            UIModule.Instance.PopUpWindow<TestStackWindow2>();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            UIModule.Instance.PopUpWindow<TestStackWindow3>();
        }
    }
}
