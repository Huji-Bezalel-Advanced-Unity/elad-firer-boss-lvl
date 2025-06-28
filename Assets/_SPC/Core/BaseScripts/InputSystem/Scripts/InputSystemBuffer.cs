using _SPC.Core.BaseScripts.Generics.MonoSingletone;

namespace _SPC.Core.BaseScripts.InputSystem.Scripts
{
    /// <summary>
    /// Singleton buffer for the game's input system, providing global access to input actions.
    /// </summary>
    public sealed class InputSystemBuffer : SpcMonoSingleton<InputSystemBuffer>
    {
        /// <summary>
        /// Gets the input system actions instance.
        /// </summary>
        public InputSystem_Actions InputSystem { get; private set; }
        
        /// <summary>
        /// Initializes the input system and enables it on awake.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            InputSystem = new InputSystem_Actions();
            InputSystem.Enable();
        }
        
        /// <summary>
        /// Disables the input system when the application quits.
        /// </summary>
        void OnApplicationQuit()
        {
            Instance.InputSystem.Disable();
        }
    }
}