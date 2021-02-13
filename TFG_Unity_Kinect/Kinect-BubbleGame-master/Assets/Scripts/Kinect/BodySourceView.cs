using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Windows.Kinect;
using Joint = Windows.Kinect.Joint;

public class BodySourceView : MonoBehaviour
{
    public BodySourceManager mBodySourceManager;
    public GameObject mJointObject;

    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();
    private List<JointType> _joints = new List<JointType>
    {
        JointType.HandLeft,
        JointType.HandRight,         
        JointType.ShoulderRight,  
        JointType.ShoulderLeft,
    };

    private Dictionary<JointType, List<Vector3>> dictCoordenadas = new Dictionary<JointType, List<Vector3>>();

    //private static List<Vector3> coordenadaMano = new List<Vector3>();  //esta lista almacena las posiciones de la mano
    public Vector3 targetPosition;
    public Vector3 shoulderposition;
    private static bool grabar = false;
    public GameObject botonGrabar;
    public GameObject botonFin;
    //public GameObject botonJugar;
   // public List<Vector3> coordenadaResultMano = new List<Vector3>();

    void Update()
    {
        #region Get Kinect data
        Body[] data = mBodySourceManager.GetData();
        if (data == null)
            return;

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
                continue;

            if (body.IsTracked)
                trackedIds.Add(body.TrackingId);
        }
        #endregion

        #region Delete Kinect bodies
        List<ulong> knownIds = new List<ulong>(mBodies.Keys);
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                // Destroy body object
                Destroy(mBodies[trackingId]);

                // Remove from list
                mBodies.Remove(trackingId);
            }
        }
        #endregion

        #region Create Kinect bodies
        foreach (var body in data)
        {
            // If no body, skip
            if (body == null)
                continue;

            if (body.IsTracked)
            {
                // If body isn't tracked, create body
                if (!mBodies.ContainsKey(body.TrackingId))
                    mBodies[body.TrackingId] = CreateBodyObject(body.TrackingId);

                // Update positions
                UpdateBodyObject(body, mBodies[body.TrackingId]);
            }
        }
        #endregion

    }

    public void CallbackBotonGrabar()
    {
        grabar = true;
        Debug.Log("Comienza a grabar el ejercicio y desaparece el boton'Grabar' ");
        botonGrabar.SetActive(false);
        botonFin.SetActive(true);
    }

    public void CallbackBotonFin()
    {
        grabar = false;
        Debug.Log("Finaliza el ejercicio y desaparece el boton'Finalizar' ");
        foreach (KeyValuePair<JointType, List<Vector3>> coordenadaGuardada in dictCoordenadas)
        {
            promedCoordinates(coordenadaGuardada.Value);

        }
        
        botonGrabar.SetActive(true);
       
    }

    private GameObject CreateBodyObject(ulong id)
    {
        Debug.Log("Dentro de create body");
        // Create body parent
        GameObject body = new GameObject("Body:" + id);

        // Create joints
        //foreach (JointType joint in _joints)
        //{
            // Create Object
            GameObject newJoint = Instantiate(mJointObject);  //
            newJoint.name = JointType.HandRight.ToString();  //donde pone JointType.HandRight antes habia variable joint del foreach

            // Parent to body
            newJoint.transform.parent = body.transform;
        //}
        return body;
    }
    
    private void UpdateBodyObject(Body body, GameObject bodyObject)
    {
        // Update joints
        foreach (JointType _joint in _joints) //por cada joint de la lista de articulaciones de arriba
        {
            // Get new target position of handRight
            Joint sourceJoint = body.Joints[JointType.HandRight]; //_joint por JointType.HandRight
            targetPosition = GetVector3FromJoint(sourceJoint);
            targetPosition.z = 0;  

            // Get joint, set new position
            Transform jointObject = bodyObject.transform.Find(JointType.HandRight.ToString());
            jointObject.position = targetPosition;

            //Get position of shoulderRight
            Joint jointShoulder = body.Joints[JointType.ShoulderRight];
            shoulderposition = GetVector3FromJoint(jointShoulder);
            shoulderposition.z = 0;
        }

        if (grabar == true)
        {
            GrabarEjercicio(body.Joints);
        }
    }

    public void GrabarEjercicio(Dictionary<JointType, Joint> dictJoints)
    {
        #region Calculo de la distancia entre coordenada actual y coordenada anterior
        //calculo de la distancia entre la posicion actual de la mano y la anterior.
        //al inicio no existe ninguna coordenada coordenadaMano.Count ==0 entonces anyade la primera 
        //despues, es cuando comienza a calcular distancias. 

        if (dictCoordenadas.Count == 0)
        {
           // dictCoordenadas[JointType.HandRight] = new List<Vector3>();
            dictCoordenadas.Add(JointType.HandRight, new List<Vector3>());
            dictCoordenadas[JointType.HandRight].Add(GetVector3FromJoint(dictJoints[JointType.HandRight]));

            dictCoordenadas.Add(JointType.ShoulderRight, new List<Vector3>());
            dictCoordenadas[JointType.ShoulderRight].Add(GetVector3FromJoint(dictJoints[JointType.ShoulderRight]));
           
            return;
        }

        float distM = Vector3.Distance(targetPosition, dictCoordenadas[JointType.HandRight][dictCoordenadas[JointType.HandRight].Count - 1]);
        float distH = Vector3.Distance(shoulderposition, dictCoordenadas[JointType.ShoulderRight][dictCoordenadas[JointType.ShoulderRight].Count - 1]);

        #endregion

        #region Imprime y anyade coordenda a la lista de coordenadas almacenadas en coordenadaMano si la distancia es mayor a 2,6(por ejemplo)

        if (distM >= 0.1 && distH >= 0.2)    //si la distancia entre la posicion actual (targetposition) y la anterior es mayr o igual a 0,6, guardala como nueva coordenada de la mano
        {

            dictCoordenadas[JointType.HandRight].Add(GetVector3FromJoint(dictJoints[JointType.HandRight]));
            dictCoordenadas[JointType.ShoulderRight].Add(GetVector3FromJoint(dictJoints[JointType.ShoulderRight]));

            foreach (KeyValuePair<JointType, List<Vector3>> coordenadaGuardada in dictCoordenadas)
            {
                foreach(Vector3 coordenada in coordenadaGuardada.Value)
                {
                }
                Debug.Log("coordenadaMano " + coordenadaGuardada);
            }

            Debug.Log("Dist" + distM); //imprimir comprobacion
            //Debug.Log(coordenadaMano.Count);  //imprimir comprobacion
         }
        #endregion
    }

    private Vector3 GetVector3FromJoint(Joint joint)
    {
        //multiplica la coordenada *10 por que ...????
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }

    #region Dividir la lista coordenadasMano en partes y coger la media de cada parte
    public void promedCoordinates(List<Vector3> listDict )
    {
        if(listDict.Count != 0)
        {
            #region CALCULO DEL PROMEDIO DE COORDENADAS DE LA MANO
            short numPartesM = 5;
            int tamanyoParte = listDict.Count / numPartesM;

            List<Vector3> coordenadaResultPromedM = new List<Vector3>();

            for (int parte = 0; parte < numPartesM; parte++)
            {
                float xParte = 0.0f, yParte = 0.0f, zParte = 0.0f;  //Agregue "f" al final para decirle al compilador que es un flotante

                for (int i = 0; i < tamanyoParte; i++)
                {
                    xParte += listDict[i + parte * tamanyoParte].x;
                    yParte += listDict[i + parte * tamanyoParte].y;
                    zParte += listDict[i + parte * tamanyoParte].z;
                }
                          
                float xPartVector = xParte / tamanyoParte;
                float yPartVector = yParte / tamanyoParte;
                float zPartVector = zParte / tamanyoParte;
                
                Debug.Log("Parte" + parte + "coordenada X" + xPartVector);
                Debug.Log("Parte" + parte + "coordenada Y" + yPartVector);
                Debug.Log("Parte" + parte + "coordenada Z" + zPartVector);


                Vector3 coordXYZ = new Vector3(xPartVector, yPartVector, zPartVector);
                coordenadaResultPromedM.Add(coordXYZ);
                             
            }
            Debug.Log(coordenadaResultPromedM);
            FileGrabacion(coordenadaResultPromedM);
            #endregion


            #region CALCULO DE LA COORDENADA DEL HOMBRO

            List<Vector3> coordenadaResultPromedH = new List<Vector3>();
            //float xParteH = 0.0f, yParteH = 0.0f, zParteH = 0.0f;  //Agregue "f" al final para decirle al compilador que es un flotante

            foreach (KeyValuePair<JointType, List<Vector3>> coordenadaGuardada in dictCoordenadas)
            {
                foreach (Vector3 coordenada in coordenadaGuardada.Value)
                {
                    //xParteH += listDict[].x;
                    //yParteH += listDict[].y;
                    //zParteH += listDict[].z;

                }
                //float xPartVector = xParte; 
                //float yPartVector = yParte;
                //float zPartVector = zParte;

                //Vector3 coordXYZ_H = new Vector3(xPartVector, yPartVector, zPartVector);
                //coordenadaResultPromedM.Add(coordXYZ);
            }
            #endregion
        }
    }
    #endregion

    #region crear txt con las coordenadas
    public void FileGrabacion(List<Vector3> listPromed)
    {
        using (StreamWriter output = new StreamWriter("Archivo.txt", true))
        {
            foreach (Vector3 argumento in listPromed) 
            {
                string margumento = argumento.ToString();
                output.WriteLine(margumento);
            }
            
            //Debug.Log("Se ha creado el archivo y anyadido " + margumento);
        }
    }
    #endregion

    //public void calculoVectorHM()
    //{
    //    //float d;
    //    //d = Mathf.Sqrt()
    //}
}
//crear los vectores de hombro-mano, hombro-hombro, cadera-cadera y de cadera-hombro
//con hombro-cadera y hombro-mano sacamos el angulo que forman 
//con hombro-hombro y cadera-cadera comprobar que se mantienen paralelas o con igual angulo que el inicial