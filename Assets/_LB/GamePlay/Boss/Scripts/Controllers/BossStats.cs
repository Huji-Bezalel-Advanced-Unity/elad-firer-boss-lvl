using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.GamePlay.Boss.Scripts.Controllers
{
    [CreateAssetMenu(menuName = "Boss/ScriptableObjects/Boss Stats")]
    public class BossStats : ScriptableObject, ILBStats
    {
        [SerializeField] private float moveSpeed = 3f;

        public float MoveSpeed => moveSpeed;
    }
}