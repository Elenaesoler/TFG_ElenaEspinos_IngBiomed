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

    private List<JointType> jointsToSave = new List<JointType>  //lista variable de joints que queremos
    {
        JointType.HandRight,
        JointType.ElbowRight,
        JointType.ShoulderRight,
    };
    Dictionary<JointType, List<Vector3>> dict = new Dictionary<JointType, List<Vector3>>();

    private Dictionary<JointType, List<Vector3>> dictCoordenadas = new Dictionary<JointType, List<Vector3>>();

    //private static List<Vector3> coordenadaMano = new List<Vector3>();  //esta lista almacena las posiciones de la mano
    public Vector3 targetPosition;
    public Vector3 shoulderposition;
    private static bool grabar = false;
    public GameObject botonGrabar;
    public GameObject botonFin;
    public GameObject botonJugar;
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
    public void CallbackBotonJugar()
    {
        botonGrabar.SetActive(false);
        botonFin.SetActive(false);
        Debug.Log("Dentro del boton Jugar");
        //ReadFile(); /* Se instancia desde unity */
        //CalcularVectores();
        //Setbubbles();

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
            promedCoordinates(coordenadaGuardada.Value, coordenadaGuardada.Key);
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
        #region comentario
        //calculo de la distancia entre la posicion actual de la mano y la anterior.
        //al inicio no existe ninguna coordenada coordenadaMano.Count ==0 entonces anyade la primera 
        //despues, es cuando comienza a calcular distancias. 
        #endregion
        // if (dictCoordenadas.Count == 0)
        //{
        foreach (JointType articulacion in jointsToSave)
            {
            //    //añadir key de que sea un joint
                if(!dictCoordenadas.ContainsKey(articulacion))
                    dictCoordenadas.Add(articulacion, new List<Vector3>());  

                dictCoordenadas[articulacion].Add(GetVector3FromJoint(dictJoints[articulacion]));
            //  
            }
           
           // return;
      //  }

        float distM = Vector3.Distance(targetPosition, dictCoordenadas[JointType.HandRight][dictCoordenadas[JointType.HandRight].Count - 1]);
        float distH = Vector3.Distance(shoulderposition, dictCoordenadas[JointType.ShoulderRight][dictCoordenadas[JointType.ShoulderRight].Count - 1]);

        #endregion

        #region Imprime y anyade coordenda a la lista de coordenadas almacenadas en coordenadaMano si la distancia es mayor a 2,6(por ejemplo)

        //if (distM >= 0.1 && distH >= 0.2)    //si la distancia entre la posicion actual y la anterior es mayr o igual a 0,6, guardala 
        //{

            foreach (KeyValuePair<JointType, List<Vector3>> coordenadaGuardada in dictCoordenadas)
            {
                foreach(Vector3 coordenada in coordenadaGuardada.Value)
                {
                }
                Debug.Log("coordenadaMano " + coordenadaGuardada);
            }
            Debug.Log("Dist" + distM); 
            //Debug.Log(coordenadaMano.Count);  
         //}
        #endregion
    }

    private Vector3 GetVector3FromJoint(Joint joint)
    {
        //multiplica la coordenada *10 por que ...????
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }

    #region Dividir la lista coordenadasMano en partes y coger la media de cada parte
    public void promedCoordinates(List<Vector3> listDict, JointType joint)
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
            FileGrabacion(coordenadaResultPromedM, joint);
            #endregion
        }
    }
    #endregion

    #region crear txt con las coordenadas
    public void FileGrabacion(List<Vector3> listPromed, JointType joint)
    {
        using (StreamWriter output = new StreamWriter("Archivo.txt", true))
        {
            output.WriteLine(((int) joint).ToString());

            foreach (Vector3 argumento in listPromed) 
            {
                string margumento = argumento.x.ToString() + ";" + argumento.y.ToString() + ";" + argumento.z.ToString();
                output.WriteLine(margumento);
            }           
        }
    }
    #endregion

    public void ReadFile()
    {
        StreamReader sr;
        string line;

        if(File.Exists("Archivo.txt"))
        {
            JointType joint = (JointType) 0;
            
            sr = new StreamReader("Archivo.txt");
            line = sr.ReadLine();

            while (line != null)
            {
                Debug.Log(line);

                if (line.Length <= 2)
                {
                    joint = (JointType)int.Parse(line);
                    dict.Add(joint, new List<Vector3>());
                }
                else
                {
                    string[] items = line.Split(';');
                    dict[joint].Add(new Vector3(float.Parse(items[0]), float.Parse(items[1]), float.Parse(items[2])));
                }

                line = sr.ReadLine();
            }
        }
        foreach (KeyValuePair<JointType, List<Vector3>> coordenaLeida in dict)
        {
            CalculoVectores(coordenaLeida.Value, coordenaLeida.Key);
        }
    }
    public Vector3 CalculoVectores(List<Vector3> listDict, JointType joint)
    {
        //calculo de los vectores directores con la lectura del diccionario resultante de la lectura del archivo 
        //Vector3 vHombroMano = new Vector3();

        foreach(KeyValuePair<JointType, List<Vector3>> listaLectura in dict) 
        {
            Debug.Log("Joint" + listaLectura.Key);
            if(listaLectura.Key == JointType.ShoulderRight)
            {
                foreach (Vector3 coordenada in listaLectura.Value)
                {
                    float coordHombrox = coordenada.x;
                    float coordHombroy = coordenada.y;
                    float coordHombroz = coordenada.z;
                   
                    Debug.Log(coordHombrox + "," + coordHombroy + "," + coordHombroz);
                    //vHombroMano  = (coordXh - coordXm, coordYh - coordYm, coordZh - coordZm)
                }
            }
            if (listaLectura.Key == JointType.ShoulderRight) 
            { 
            }
                
        }

        Vector3 a = new Vector3(1, 2, 3);
        return a ;
    }  
}
