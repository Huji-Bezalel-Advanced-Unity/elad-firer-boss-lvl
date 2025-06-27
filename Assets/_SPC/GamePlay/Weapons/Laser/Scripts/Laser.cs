
using UnityEngine;

namespace _SPC.GamePlay.Weapons
{
    public class LaserContainer 
    {
        public Laser Laser;
        public BoxCollider2D BoxCollider;
        public LineRenderer Lr;
        public Transform FromT, ToT;
        public Vector3 FromLocalPos, ToLocalPos;
    }
    public class Laser: SPCBaseWeapon
    {
        [Header("Laser")] [SerializeField] private float laserTimeHurt = 0.5f;
        private float _lastHitTime;
        private LaserContainer _laserContanier;
        [InspectorButton]
        public void UpdateBoxCollider()
        {
            if (_laserContanier?.FromT == null || _laserContanier?.ToT == null) return;
            
            Vector2 fromPos = _laserContanier.FromT.position;
            Vector2 toPos = _laserContanier.ToT.position;
            Vector2 direction = (toPos - fromPos).normalized;
            float distance = Vector2.Distance(fromPos, toPos);
            
            _laserContanier.BoxCollider.size = new Vector2(distance, _laserContanier.Lr.endWidth); 
            _laserContanier.BoxCollider.transform.position = (fromPos + toPos) * 0.5f;
            _laserContanier.BoxCollider.offset = Vector2.zero;
           
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _laserContanier.BoxCollider.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        public void Update()
        {
            UpdateLaserPositions();
        }
        
        [InspectorButton]
        private void UpdateLaserPositions()
        {
            _laserContanier.Lr.SetPosition(0, _laserContanier.FromT.TransformPoint(_laserContanier.FromLocalPos));
            _laserContanier.Lr.SetPosition(1, _laserContanier.ToT.TransformPoint(_laserContanier.ToLocalPos));
        }

        public override void OnTriggerEnter2D(Collider2D other)
        {
            _hitTransform = other.transform;
            base.OnTriggerEnter2D(other);
            if (_hitSuccess)
            {
                _lastHitTime = Time.time;
            }
        }

        public void OnTriggerStay2D(Collider2D other)
        {
            if (_target != null && Time.time - _lastHitTime > laserTimeHurt)
            {
                _target.GotHit(other.transform.position,_weaponType);
            }
        }
        
        public static LaserContainer CreateLaser(GameObject laserPrefab, Transform parent, GameObject fromGo, GameObject toGo)
        {
            var laserObject = Instantiate(laserPrefab, parent);
            laserObject.name = "Laser";
            
            laserObject.transform.localPosition = Vector3.zero;
            laserObject.transform.localRotation = Quaternion.identity;
            laserObject.transform.localScale = Vector3.one;

            
            var lr = laserObject.GetComponentInChildren<LineRenderer>();
            if (lr == null)
                Debug.LogError(new System.Exception("line renderer not found"));
            var box = laserObject.GetComponentInChildren<BoxCollider2D>();
            if (box == null)
                Debug.LogError(new System.Exception("Box Collider not found"));
            var laser = laserObject.GetComponentInChildren<Laser>();
            if (laser == null)
                Debug.LogError(new System.Exception("Laser not found"));
            lr.useWorldSpace = true;
            lr.positionCount = 2;

            var fromT = fromGo.transform;
            var toT = toGo.transform;
            Vector3 fromLocal = fromT.InverseTransformPoint(fromT.position);
            Vector3 toLocal = toT.InverseTransformPoint(toT.position);

            lr.SetPosition(0, fromT.position);
            lr.SetPosition(1, toT.position);

            laser._laserContanier = new LaserContainer
            {
                Laser = laser,
                BoxCollider = box,
                Lr = lr,
                FromT = fromT,
                ToT = toT,
                FromLocalPos = fromLocal,
                ToLocalPos = toLocal
            };
            return laser._laserContanier;
        }
        
    }
}