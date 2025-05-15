using UnityEngine;

namespace _SPC.GamePlay.Weapons.Bullet
{
    public class ParticleSystemDestroy : MonoBehaviour {

        // Use this for initialization
        void Start () {
            Destroy(gameObject, GetComponent<ParticleSystem>().duration);
        }
        
    }
}