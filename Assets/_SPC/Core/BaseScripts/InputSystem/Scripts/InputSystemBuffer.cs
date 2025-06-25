using _SPC.Core.BaseScripts.Generics.MonoSingletone;

namespace _SPC.Core.BaseScripts.InputSystem.Scripts
{
    public sealed class InputSystemBuffer: SpcMonoSingleton<InputSystemBuffer>
    {
        

        // Private ctor prevents external instantiation
        protected override void Awake()
        {
            base.Awake();
            InputSystem = new InputSystem_Actions();
            InputSystem.Enable();
        }
        
        public InputSystem_Actions InputSystem { get; private set; }
        
        void OnApplicationQuit()
        {
            InputSystemBuffer.Instance.InputSystem.Disable();
        }
    }
}