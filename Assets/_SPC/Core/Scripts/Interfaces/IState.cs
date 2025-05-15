namespace _SPC.Core.Scripts.Interfaces
{
    public interface IState
    {
        public void Enter();
        public void Update();
        public void Exit();
    }
}