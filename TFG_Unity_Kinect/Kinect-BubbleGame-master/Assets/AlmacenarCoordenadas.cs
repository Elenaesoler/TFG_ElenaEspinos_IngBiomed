using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using Windows.Kinect;
using Joint = Windows.Kinect.Joint;

public class AlmacenarCoordenadas : MonoBehaviour
{
    //// Suelta este script debajo de un objeto en tu escena y especifica otros 2 objetos en las variables "startMarker" / "endMarker" en la ventana del inspector de script.
    //// En el momento de la reproducción, el script moverá el objeto a lo largo de una ruta entre la posición de esos dos marcadores. 
    ////https://docs.unity3d.com/ScriptReference/Vector3.Lerp.html

    //// Transforms to act as start and end markers for the journey.
    public Transform startMarker; //coordenada de inicio
    public Transform endMarker;  //coordenada final

    // Movement speed in units per second.
    public float speed = 2000.0F;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("entro en start de almacenarCoordenadas");
        //meter un timestamp
        // boolean de captura = true;
        // variable de guardar joints. Llama a la lista del script bodysourceview
        // Keep a note of the time the movement started.
        startTime = Time.time;

        // Calculate the journey length.
        journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
    }

    // Update is called once per frame
    void Update()
    {
        //timestampN
        // if(timesatmpN - Timestamp > x) and  captura==true
        //      guardar coordenada_mano (coordenadaMano.Add)
        //       if(dist < x) (si la distancia es minima que apenas se mueve la mano)
        //                captura ==false;

        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * speed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;

        // Set our position as a fraction of the distance between the markers.
        transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney);
    }

    #region TIMER
    //public Text tiempoText;
    //public float tiempo = 0.0f;

    //public void CountDown()
    //{
    //   tiempo = tiempo - 1 * Time.dataTime;  // para que vaya acorde al tiempo real decreciendo
    //    tiempoText.text = "" + tiempo.ToString("f0");
    //}




    #endregion
    //public int interpolationFramesCount = 1800; // Number of frames to completely interpolate between the 2 positions
    //int elapsedFrames = 0;

    //void Update()
    //{
    //    float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;

    //    Vector3 interpolatedPosition = Vector3.Lerp(Vector3.up, Vector3.forward, interpolationRatio);

    //    elapsedFrames = (elapsedFrames + 1) % (interpolationFramesCount + 1);  // reset elapsedFrames to zero after it reached (interpolationFramesCount + 1)

    //    Debug.DrawLine(Vector3.zero, Vector3.up, Color.green);
    //    Debug.DrawLine(Vector3.zero, Vector3.forward, Color.blue);
    //    Debug.DrawLine(Vector3.zero, interpolatedPosition, Color.yellow);
    //}
}
