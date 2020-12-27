using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hand : MonoBehaviour
{
    public Transform mHandMesh;
   // public List<Bubble> explotedBubbles = new List<Bubble>(); //Lista de burbujas ya explotadas
    public int explotedBubble;

    private void Update()
    {
        mHandMesh.position = Vector3.Lerp(mHandMesh.position, transform.position, Time.deltaTime * 15.0f);
    }

    public void Cambio1(string sceneName) //metodo de cambio de escena al acabar 
    {
        SceneManager.LoadScene(sceneName);
    }

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

        if(explotedBubble==10)   //cuando se exploten 40 (por ejemplo, se puede cambiar. CAMBIAR A VARIABLE DESDE UNITY) pompas acaba el juego. 
            Cambio1("MenuScene");
        




    }
}
