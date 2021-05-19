using UnityEngine;

namespace ObjectScripts
{
    public class CoinRotationScript : MonoBehaviour
    {

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(new Vector3(0,50*Random.value,0)*Time.deltaTime);
        }
    }
}
