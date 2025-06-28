using UnityEngine;

namespace _SPC.GamePlay.Weapons
{
    /// <summary>
    /// Container class that holds all components needed for a laser weapon.
    /// Provides easy access to laser components and their transforms.
    /// </summary>
    public class LaserContainer 
    {
        public Laser Laser;
        public BoxCollider2D BoxCollider;
        public LineRenderer Lr;
        public Transform FromT, ToT;
        public Vector3 FromLocalPos, ToLocalPos;
    }

    /// <summary>
    /// Laser weapon that creates a continuous beam between two points.
    /// Handles collision detection, visual rendering, and damage application over time.
    /// </summary>
    public class Laser : SPCBaseWeapon
    {
        [Header("Laser Configuration")]
        [Tooltip("Time interval between damage applications when target stays in laser beam.")]
        [SerializeField] private float _laserTimeHurt = 0.5f;

        [Header("Laser State")]
        [Tooltip("Time of the last damage application.")]
        private float _lastHitTime;
        
        [Tooltip("Container holding all laser components and references.")]
        private LaserContainer _laserContainer;

        /// <summary>
        /// Updates the box collider to match the laser beam's current position and orientation.
        /// Called via Inspector button for manual updates.
        /// </summary>
        [InspectorButton]
        public void UpdateBoxCollider()
        {
            if (_laserContainer?.FromT == null || _laserContainer?.ToT == null) 
            {
                Debug.LogWarning("Cannot update box collider: FromT or ToT is null.");
                return;
            }
            
            Vector2 fromPos = _laserContainer.FromT.position;
            Vector2 toPos = _laserContainer.ToT.position;
            Vector2 direction = (toPos - fromPos).normalized;
            float distance = Vector2.Distance(fromPos, toPos);
            
            _laserContainer.BoxCollider.size = new Vector2(distance, _laserContainer.Lr.endWidth); 
            _laserContainer.BoxCollider.transform.position = (fromPos + toPos) * 0.5f;
            _laserContainer.BoxCollider.offset = Vector2.zero;
           
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _laserContainer.BoxCollider.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        /// <summary>
        /// Updates laser positions every frame to maintain visual accuracy.
        /// </summary>
        public void Update()
        {
            UpdateLaserPositions();
        }
        
        /// <summary>
        /// Updates the line renderer positions to match the current from/to transforms.
        /// Called via Inspector button for manual updates.
        /// </summary>
        [InspectorButton]
        private void UpdateLaserPositions()
        {
            if (_laserContainer?.FromT == null || _laserContainer?.ToT == null) return;

            _laserContainer.Lr.SetPosition(0, _laserContainer.FromT.TransformPoint(_laserContainer.FromLocalPos));
            _laserContainer.Lr.SetPosition(1, _laserContainer.ToT.TransformPoint(_laserContainer.ToLocalPos));
        }

        /// <summary>
        /// Handles initial collision detection when a target enters the laser beam.
        /// Records the hit time for continuous damage application.
        /// </summary>
        /// <param name="other">The collider that entered the laser beam.</param>
        public override void OnTriggerEnter2D(Collider2D other)
        {
            _hitTransform = other.transform;
            base.OnTriggerEnter2D(other);
            if (_hitSuccess)
            {
                _lastHitTime = Time.time;
            }
        }

        /// <summary>
        /// Handles continuous collision detection while a target stays in the laser beam.
        /// Applies damage at regular intervals based on laserTimeHurt.
        /// </summary>
        /// <param name="other">The collider that is staying in the laser beam.</param>
        public void OnTriggerStay2D(Collider2D other)
        {
            if (_target != null && Time.time - _lastHitTime > _laserTimeHurt)
            {
                _target.GotHit(other.transform.position, _weaponType);
                _lastHitTime = Time.time;
            }
        }
        
        /// <summary>
        /// Creates a new laser instance with all necessary components and configurations.
        /// </summary>
        /// <param name="laserPrefab">The laser prefab to instantiate.</param>
        /// <param name="parent">Parent transform for the laser object.</param>
        /// <param name="fromGo">GameObject representing the laser start point.</param>
        /// <param name="toGo">GameObject representing the laser end point.</param>
        /// <returns>LaserContainer with all configured components and references.</returns>
        public static LaserContainer CreateLaser(GameObject laserPrefab, Transform parent, GameObject fromGo, GameObject toGo)
        {
            if (laserPrefab == null || parent == null || fromGo == null || toGo == null)
            {
                Debug.LogError("CreateLaser: One or more required parameters are null.");
                return null;
            }

            var laserObject = Instantiate(laserPrefab, parent);
            laserObject.name = "Laser";
            
            laserObject.transform.localPosition = Vector3.zero;
            laserObject.transform.localRotation = Quaternion.identity;
            laserObject.transform.localScale = Vector3.one;

            var lr = laserObject.GetComponentInChildren<LineRenderer>();
            if (lr == null)
            {
                Debug.LogError("CreateLaser: LineRenderer component not found in laser prefab.");
                return null;
            }

            var box = laserObject.GetComponentInChildren<BoxCollider2D>();
            if (box == null)
            {
                Debug.LogError("CreateLaser: BoxCollider2D component not found in laser prefab.");
                return null;
            }

            var laser = laserObject.GetComponentInChildren<Laser>();
            if (laser == null)
            {
                Debug.LogError("CreateLaser: Laser component not found in laser prefab.");
                return null;
            }

            lr.useWorldSpace = true;
            lr.positionCount = 2;

            var fromT = fromGo.transform;
            var toT = toGo.transform;
            Vector3 fromLocal = fromT.InverseTransformPoint(fromT.position);
            Vector3 toLocal = toT.InverseTransformPoint(toT.position);

            lr.SetPosition(0, fromT.position);
            lr.SetPosition(1, toT.position);

            laser._laserContainer = new LaserContainer
            {
                Laser = laser,
                BoxCollider = box,
                Lr = lr,
                FromT = fromT,
                ToT = toT,
                FromLocalPos = fromLocal,
                ToLocalPos = toLocal
            };

            return laser._laserContainer;
        }
    }
}