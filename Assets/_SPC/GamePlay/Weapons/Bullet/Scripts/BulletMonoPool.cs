using _SPC.Core.BaseScripts.Generics.MonoPool;

namespace _SPC.GamePlay.Weapons.Bullet
{
    /// <summary>
    /// Object pool for managing bullet instances to improve performance.
    /// Reuses bullet objects instead of creating and destroying them.
    /// </summary>
    public class BulletMonoPool : SPCMonoPool<Bullet>
    {
        // Inherits all functionality from SPCMonoPool<Bullet>
        // Provides type-safe bullet pooling with automatic lifecycle management
    }
}