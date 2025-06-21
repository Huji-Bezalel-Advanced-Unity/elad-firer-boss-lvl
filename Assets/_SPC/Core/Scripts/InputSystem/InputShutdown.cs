using _SPC.Core.Scripts.Generics;
using UnityEngine;

namespace _SPC.Core.Scripts.InputSystem
{
    public class InputShutdown : SpcMonoSingleton<MonoBehaviour>
    {
        void OnApplicationQuit()
        {
            InputSystemBuffer.Instance.InputSystem.Disable();
        }
    }
}