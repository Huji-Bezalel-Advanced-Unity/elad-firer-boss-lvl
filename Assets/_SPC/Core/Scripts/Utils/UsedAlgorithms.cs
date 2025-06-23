using System.Collections.Generic;
using UnityEngine;

namespace _SPC.Core.Scripts.Utils
{
    public static class UsedAlgorithms
    {
        /// <summary>
        /// Finds and returns the closest target to the given entity from a list of potential targets.
        /// </summary>
        /// <param name="targets">A list of potential target transforms.</param>
        /// <param name="entityTransform">The transform of the entity to measure distances from.</param>
        /// <returns>The transform of the closest target, or null if the list is empty or contains no valid targets.</returns>
        public static Transform GetClosestTarget(List<Transform> targets, Transform entityTransform)
        {
            Transform closest = null;
            float shortestDistance = float.MaxValue;

            foreach (Transform target in targets)
            {
                if (target == null) continue;

                float distance = Vector2.Distance(entityTransform.position, target.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closest = target;
                }
            }

            return closest;
        }
    }
}