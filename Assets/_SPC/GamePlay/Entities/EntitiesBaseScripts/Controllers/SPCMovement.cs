namespace _SPC.GamePlay.Entities
{
    public abstract class SPCMovement
    {
        public bool IsMoving { get; protected set; }
        public abstract void UpdateMovement();

        public abstract void Cleanup();
    }
}