using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float projectileSpeed;
    public float sniperRange;
    private Transform _ownTransform;
    void Start()
    {
        _ownTransform = transform;
        Destroy(gameObject, sniperRange / projectileSpeed); //Zerstört die Kugel sobald sie außer Reichweite ist
       
    }

    // Update is called once per frame
    void Update()
    {
        _ownTransform.position += _ownTransform.forward * (Time.deltaTime * projectileSpeed); //Schießt in richtung die er guckt
    }
}
