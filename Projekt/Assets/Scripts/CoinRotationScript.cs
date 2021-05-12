using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRotationScript : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3((float) -6.740*2,(float) -8.189*2,(float) 11.12*2)*Time.deltaTime);
    }
}
