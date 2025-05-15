using UnityEngine;

namespace _LB.GamePlay.Player.Scripts.Weapon
{
    public class ParticleSystemDestroy : MonoBehaviour {

        // Use this for initialization
        void Start () {
            Destroy(gameObject, GetComponent<ParticleSystem>().duration);
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}