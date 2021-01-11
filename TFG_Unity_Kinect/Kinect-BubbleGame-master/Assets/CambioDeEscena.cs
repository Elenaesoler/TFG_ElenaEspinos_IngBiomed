using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;   //para poder trabajar con distintas escenas (manejo de escenas)

public class CambioDeEscena : MonoBehaviour
{
    //private float waitTime;  //introduce en unity el tiempo a mano
    //private float timer = 0.0f;

    //public void awake()
    //{

    //}
    //public void timer()
    //{

    //}
    public void Cambio(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}