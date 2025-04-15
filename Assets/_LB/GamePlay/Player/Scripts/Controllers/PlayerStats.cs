using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.GamePlay.Player.Scripts.Controllers
{
    [CreateAssetMenu(menuName = "Player/ScriptableObjects/Player Stats")]
    public class PlayerStats : ScriptableObject, ILBStats
    {
        [SerializeField] private float moveSpeed = 5f;

        public float MoveSpeed => moveSpeed;
    
    }
}