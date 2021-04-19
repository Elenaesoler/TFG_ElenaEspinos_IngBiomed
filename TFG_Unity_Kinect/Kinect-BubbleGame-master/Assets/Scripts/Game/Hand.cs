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

    ///// <summary>
    ///// Draws a hand symbol if the hand is tracked: red circle = closed, green circle = opened; blue circle = lasso
    ///// </summary>
    ///// <param name="handState">state of the hand</param>
    ///// <param name="handPosition">position of the hand</param>
    ///// <param name="drawingContext">drawing context to draw to</param>
    //private void DrawHand(HandState handState, Point handPosition, DrawingContext drawingContext)
    //{
    //    switch (handState)
    //    {
    //        case HandState.Closed:
    //            drawingContext.DrawEllipse(this.handClosedBrush, null, handPosition, HandSize, HandSize);
    //            break;

    //        case HandState.Open:
    //            drawingContext.DrawEllipse(this.handOpenBrush, null, handPosition, HandSize, HandSize);
    //            break;

    //        case HandState.Lasso:
    //            drawingContext.DrawEllipse(this.handLassoBrush, null, handPosition, HandSize, HandSize);
    //            break;
    //    }
    //}
}