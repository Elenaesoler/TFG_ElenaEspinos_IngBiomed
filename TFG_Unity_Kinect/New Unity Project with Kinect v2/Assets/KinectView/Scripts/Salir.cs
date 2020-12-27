using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;   //para poder trabajar con distintas escenas (manejo de escenas)

public class Salir : MonoBehaviour
{
    public void SalirAlMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
