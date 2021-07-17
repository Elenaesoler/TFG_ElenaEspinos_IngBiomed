using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReproducirJuego : MonoBehaviour
{
    public void Cambio(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
