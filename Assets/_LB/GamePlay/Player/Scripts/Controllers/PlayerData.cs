using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Interfaces;
using Core.Input_System;
using UnityEngine;

namespace _LB.GamePlay.Player.Scripts.Controllers
{
    public class PlayerData: LBData
    {
        

        public PlayerData(Collider2D collider, LBStats stats) : base(collider, stats)
        {
            
        }

        
    }
}