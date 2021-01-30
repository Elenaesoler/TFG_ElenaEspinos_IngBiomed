using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circular : MonoBehaviour
{
    public GameObject prefab;
    public GameObject Mano;
    public GameObject Hombro;

    public int numberOfObjects = 20;
    public float radius = 5f;

    //Necesito las coordenadas del HOMBRO, CODO Y MUNYECA/MANO
    //L --Longitud del brazo -- Suma de Vector3.mano, Vector3.codo y Vector3.Hombro
    // centro del arco -- coordenada del hombro 

    //float coordenadaMano = Hombro(Transform.position);
    //float coordenadaHombro = Transform.position;

    
    void Start()
    {
        //var float long = new Vector3(Mano.position - Hombro.position);
        for (int i =0; i< numberOfObjects; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfObjects;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle),0) * radius;
            Instantiate(prefab, pos, Quaternion.identity);

        }
        
    }
}
