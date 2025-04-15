using _LB.Core.Scripts.Abstracts;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.GamePlay.Player.Scripts.Controllers
{
    public class PlayerData: LBData
    {
        public PlayerData(Collider2D collider, ILBStats stats) : base(collider, stats)
        {
        }
    }
}