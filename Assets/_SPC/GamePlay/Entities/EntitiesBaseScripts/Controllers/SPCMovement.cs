namespace _SPC.GamePlay.Entities
{
    /// <summary>
    /// Base class for all movement systems, providing common functionality for entity movement.
    /// </summary>
    public abstract class SPCMovement
    {
        /// <summary>
        /// Gets or sets whether the entity is currently moving.
        /// </summary>
        public bool IsMoving { get; protected set; }

        /// <summary>
        /// Updates the movement logic. Must be implemented by derived classes.
        /// </summary>
        public abstract void UpdateMovement();

        /// <summary>
        /// Cleans up resources when the movement system is destroyed. Must be implemented by derived classes.
        /// </summary>
        public abstract void Cleanup();
    }
}