using _LB.Core.Scripts.Abstracts;
using UnityEngine;

namespace _LB.GamePlay.Player.Scripts.Controllers
{
    public class PlayerMovement: LBMovement
    {
        public PlayerMovement(Rigidbody2D rb) : base(rb)
        {
        }

        public override void UpdateMovement()
        {
            throw new System.NotImplementedException();
        }
    }
}