using UnityEngine;

namespace ObjectScripts
{
    public class RotationScript : MonoBehaviour
    {
        public float rotationSpeed = 50;
        void Update()
        {
            transform.Rotate(new Vector3(0,rotationSpeed*Random.value,0)*Time.deltaTime);
        }
    }
}
