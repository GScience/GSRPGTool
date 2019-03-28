using System.Collections;
using System.Collections.Generic;
using RPGTool.System;
using UnityEngine;

public class Doordisk : Subwindow
{
    public RotableDisk disk1;
    public RotableDisk disk2;

    protected override void Update()
    {
        base.Update();
        if (Mathf.Abs(disk1.transform.rotation.z) <= 0.1f && Mathf.Abs(disk2.transform.rotation.z) <= 0.1f)
        {
            result = true;
            Close();
        }
    }
}
