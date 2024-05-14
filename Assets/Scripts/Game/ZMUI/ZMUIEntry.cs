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
    }
}
