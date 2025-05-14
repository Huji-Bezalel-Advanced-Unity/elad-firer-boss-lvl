using _LB.Core.Scripts.Generics;
using Core.Input_System;
using UnityEngine;

namespace _LB.Core.Scripts.InputSystem
{
    public class InputShutdown : LBMonoSingleton<MonoBehaviour>
    {
        void OnApplicationQuit()
        {
            InputSystemBuffer.Instance.InputSystem.Disable();
        }
    }
}