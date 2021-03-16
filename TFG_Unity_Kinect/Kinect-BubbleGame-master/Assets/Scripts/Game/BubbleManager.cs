using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    public GameObject mBubblePrefab;    //TIPO GAMEOBJECT 
    //public GameObject botonJugar;
    public static List<Vector3> listaPos = new List<Vector3>();

    private List<Bubble> mAllBubbles = new List<Bubble>();      //LISTA de burbujas 
    private Vector2 mBottomLeft = Vector2.zero;   //
    private Vector2 mTopRight = Vector2.zero;   // 
    
    private void Awake()
    {
        // Bounding values
        Debug.Log("Entro en awake - Bubble manager");
        mBottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.farClipPlane));
        mTopRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight / 2, Camera.main.farClipPlane));
    }

    private void Start()
    {
        Debug.Log("Empieza start y el juego comienza a capturar coordenadas - Bubble manager");
        StartCoroutine(CreateBubbles());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.farClipPlane)), 0.5f);
        Gizmos.DrawSphere(Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, Camera.main.farClipPlane)), 0.5f);
    }

    public static void setListaPos(List<Vector3> lista)
    {
        listaPos = lista;
    }


    public Vector3 GetPlanePosition()
    {
        float targetX = Random.Range(mBottomLeft.x, mTopRight.x);
        float targetY = Random.Range(mBottomLeft.y, mTopRight.y);

        return new Vector3(targetX, targetY, 0);
    }
    
    //private IEnumerator crearPompas()
    //{
    //  while (gameObject = true)
    //      crea la burbuja
    //      Cuando haya creado las 5
    //      gameObject = false

    private IEnumerator CreateBubbles()
    {
        Debug.Log("empieza creat bubble");
        while (mAllBubbles.Count < 7)
        {
            // Create and add
            GameObject newBubbleObject = Instantiate(mBubblePrefab, GetPlanePosition(), Quaternion.identity, transform);
            Bubble newBubble = newBubbleObject.GetComponent<Bubble>();

            // Setup bubble
            newBubble.mBubbleManager = this;
            mAllBubbles.Add(newBubble);

            yield return new WaitForSeconds(0.5f);
        }
    }
    
}
