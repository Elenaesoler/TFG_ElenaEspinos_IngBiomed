using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Windows.Kinect;
using Joint = Windows.Kinect.Joint;

public class Hand : MonoBehaviour
{
    public Transform mHandMesh;
    public int explotedBubble;
    private HandState handState;
    private SpriteRenderer mSpriteRenderer = null;
    public Sprite T_Hand;
    public Sprite T_Cursor;


    private void Awake()
    {
        mSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    //la coordenada de la mano se actualiza cada 15.0 frames 
    private void Update()
    {
        mHandMesh.position = Vector3.Lerp(mHandMesh.position, transform.position, Time.deltaTime * 15.0f);
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

        //if(explotedBubble==10)   //cuando se exploten 10 (por ejemplo, se puede cambiar. CAMBIAR A VARIABLE DESDE UNITY) pompas acaba el juego. 
        //    Cambio1("MenuScene");
    }

    public void updateState(HandState state)
    {
        Debug.Log("state update: " + state.ToString());
        if (handState != state)
        {
            handState = state;
            if (handState == HandState.Closed) 
            {
                mSpriteRenderer.sprite = T_Cursor;
               // if(!collision.gameObject.CompareTag("Bubble"))
            }
            else// if (handState == HandState.Open)
            {
                mSpriteRenderer.sprite = T_Hand;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (handState == HandState.Closed) 
        {
            if (collision.gameObject.CompareTag("boton")) 
            {
                //collision.gameObject.GetComponent<>();
                GameObject boton = collision.gameObject.GetComponent<GameObject>();
                //Click();
                
            }
        }
    }

    ///// Draws a hand symbol if the hand is tracked: red circle = closed, green circle = opened; blue circle = lasso
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