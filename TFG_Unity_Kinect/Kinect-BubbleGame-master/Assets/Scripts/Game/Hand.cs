using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hand : MonoBehaviour
{
    public Transform mHandMesh;
    public int explotedBubble;

    //la coordenada de la mano se actualiza cada 15.0 frames 
    private void Update()
    {
        mHandMesh.position = Vector3.Lerp(mHandMesh.position, transform.position, Time.deltaTime * 15.0f);
    }

    
    //el metodo cambio1 sirve para finalizar un ejercicio, nivel o escena. 
    //al invocar el metodo cambio1, elegimos a que otra escena queremos ir
    public void Cambio1(string sceneName) //metodo de cambio de escena al acabar 
    {
        SceneManager.LoadScene(sceneName);
    }

    //en este metodo, cuando hay colision, se incrementa la variable explotedBuble
    // al llegar al numero de pompas que se quiera, se invoca al metodo "cambio1"

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Bubble"))
            //ESTO NO ES COLISION

            return;

        explotedBubble = explotedBubble + 1;   //la variable almacena las pompas que explotan
        Debug.Log("pop" + explotedBubble.ToString()); //imprime en consola el numero de colision MIRAR POR QUE SE DUPLICAN
                                                        //ToString 
        Bubble bubble = collision.gameObject.GetComponent<Bubble>();
        StartCoroutine(bubble.Pop());

        if(explotedBubble==10)   //cuando se exploten 10 (por ejemplo, se puede cambiar. CAMBIAR A VARIABLE DESDE UNITY) pompas acaba el juego. 
            Cambio1("MenuScene");

    }
}
