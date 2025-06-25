using UnityEngine;

namespace _SPC.GamePlay.Weapons
{
    public static class LaserCreator
    {
        public class LaserSegment
        {
            public BoxCollider2D BoxCollider;
            public LineRenderer Lr;
            public Transform FromT, ToT;
            public Vector3 FromLocalPos, ToLocalPos;
        }

        public static LaserSegment CreateLaser(GameObject laserPrefab, Transform parent, GameObject fromGo, GameObject toGo)
        {
            var laserObject = Object.Instantiate(laserPrefab, parent);
            laserObject.name = "Laser";
            
            laserObject.transform.localPosition = Vector3.zero;
            laserObject.transform.localRotation = Quaternion.identity;
            laserObject.transform.localScale = Vector3.one;

            
            var lr = laserObject.GetComponent<LineRenderer>();
            if (lr == null)
                lr = laserObject.AddComponent<LineRenderer>();
            var box = laserObject.GetComponent<BoxCollider2D>();
            if (box == null)
                box = laserObject.AddComponent<BoxCollider2D>();
            lr.useWorldSpace = true;
            lr.positionCount = 2;

            
            var fromT = fromGo.transform;
            var toT = toGo.transform;
            Vector3 fromLocal = fromT.InverseTransformPoint(fromT.position);
            Vector3 toLocal = toT.InverseTransformPoint(toT.position);

            lr.SetPosition(0, fromT.position);
            lr.SetPosition(1, toT.position);

            return new LaserSegment
            {
                BoxCollider = box,
                Lr = lr,
                FromT = fromT,
                ToT = toT,
                FromLocalPos = fromLocal,
                ToLocalPos = toLocal
            };
        }
    }
}