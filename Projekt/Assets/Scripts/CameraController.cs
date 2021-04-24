using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class CameraController : MonoBehaviour
{
    public GameObject player;
    public Vector2 zoomRange = new Vector2(4.0f, 12.0f); //Vector, der angibt, dass von Distanz 4(x) bis 12(y) gezoomt sein kann.
    public int currentZoom = 8; //Startwert 8;
    public int defaultFOV = 60;
    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        //Fixwerte würden auch gehen, aber so passt es sich mit currentZoom an.
        offset = new Vector3(0.0f, currentZoom, -currentZoom);
    }

    // Update is called once per frame
    void OnZoom(InputValue movementValue)
    {
        Vector2 zoomVector = movementValue.Get<Vector2>();
        if (zoomVector.y < 0)
        {
            //Reinzoomen
            if (currentZoom <= zoomRange.y)
            {
                currentZoom++;
            }
        }
        else
        {
            //Rauszoomen
            if (currentZoom >= zoomRange.x)
            {
                currentZoom--;
            }
        }
        //Neuberechnung des Offsets
        offset = new Vector3(0.0f, currentZoom, -currentZoom);
        //Debug.Log(player.transform.position);
    }
    void LateUpdate() //LateUpdate für Updates die was anzeigen sollen, da die als letztes berechnet werden
    {
        //Position der Kamera
        transform.position = player.transform.position + offset;
        
        //Field Of View (To Be Removed - sieht nicht so schön aus)
        Vector3 ballVelocity = player.GetComponent<Rigidbody>().velocity;
        //Da ich speed aus der RigidBody info nicht auslesen konnte, habe ich aus der Velocity die Speed berechnet (Vektorbetrag = Länge = Speed)
        float ballSpeed = Mathf.Sqrt(Mathf.Pow(ballVelocity.x,2.0f)+Mathf.Pow(ballVelocity.y,2.0f)+Mathf.Pow(ballVelocity.z,2.0f));
        Camera.main.fieldOfView = defaultFOV+ballSpeed/2; //TODO: Globale Referenz auf genau dieses Objekt, muss geändert werden
    }
}
