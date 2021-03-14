﻿using UnityEngine;
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
    private int numBodies = 0;

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

    private Dictionary<JointType, Joint> currentJoint = new Dictionary<JointType, Joint>();

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
        numBodies = 0;
        //Debug.Log(numBodies);
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
                numBodies += 1;
                // If body isn't tracked, create body
                if (!mBodies.ContainsKey(body.TrackingId))
                    mBodies[body.TrackingId] = CreateBodyObject(body.TrackingId);

                // Update positions
                currentJoint = body.Joints;
                UpdateBodyObject(body, mBodies[body.TrackingId]);
            }
        }
        #endregion

    }
    public void CallbackBotonJugar()
    {
        if (numBodies > 0)
        {
            botonGrabar.SetActive(false);
            botonFin.SetActive(false);
            botonJugar.SetActive(false);
            Debug.Log("Dentro del boton Jugar");
            Dictionary<JointType, List<Vector3>> dict = ReadFile(); /* Se instancia desde unity */
            List<Vector3> CP = CalculoVectores(dict);

            //Setbubbles(CalculoVectores(ReadFile()));
        }
    }
    public void CallbackBotonGrabar()
    {
        grabar = true;
        Debug.Log("Comienza a grabar el ejercicio y desaparece el boton'Grabar' ");
        botonGrabar.SetActive(false);
        botonJugar.SetActive(false);
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
        botonJugar.SetActive(true);
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
            //Joint jointShoulder = body.Joints[JointType.ShoulderRight];
            //shoulderposition = GetVector3FromJoint(jointShoulder);
            //shoulderposition.z = 0;
        }

        if (grabar == true)
        {
            GrabarEjercicio(body.Joints);
        }
    }

    public void GrabarEjercicio(Dictionary<JointType, Joint> dictJoints)
    {
        #region Calculo de la distancia entre coordenada actual y coordenada anterior
        
        // if (dictCoordenadas.Count == 0)
        //{
        foreach (JointType articulacion in jointsToSave)
        {
            //añadir key de que sea un joint
            if(!dictCoordenadas.ContainsKey(articulacion))
                dictCoordenadas.Add(articulacion, new List<Vector3>());  

            dictCoordenadas[articulacion].Add(GetVector3FromJoint(dictJoints[articulacion]));
        }

        // return;
        //  }

        //float distM = Vector3.Distance(targetPosition, dictCoordenadas[JointType.HandRight][dictCoordenadas[JointType.HandRight].Count - 1]);
        //float distH = Vector3.Distance(shoulderposition, dictCoordenadas[JointType.ShoulderRight][dictCoordenadas[JointType.ShoulderRight].Count - 1]);
        
        //foreach (KeyValuePair<JointType, List<Vector3>> coordenadaGuardada in dictCoordenadas)
        //{
        //    foreach(Vector3 coordenada in coordenadaGuardada.Value)
        //    {
        //    }
        //} 
        #endregion
    }

    private Vector3 GetVector3FromJoint(Joint joint)
    {
        //multiplica la coordenada *10 
        //las coordenadas en kinect se expresan de 0 a 1 (+ y -)
        //las coordenadas en el mundo unity van de 0 a 100 (+ y -)
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

    #region Leer archivo txt y añade el contenido a un dicccionario
    public Dictionary<JointType, List<Vector3>> ReadFile()
    {
        Dictionary<JointType, List<Vector3>> dict = new Dictionary<JointType, List<Vector3>>();
        StreamReader sr;
        string line;

        if(File.Exists("Archivo.txt"))
        {
            JointType joint = (JointType) 0;
            
            sr = new StreamReader("Archivo.txt");
            line = sr.ReadLine();

            while (line != null)
            {
                //Debug.Log(line);

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
        return dict;
    }
    #endregion

    #region Ordenar lista de joints crecientemente
    public List<JointType> bubbleSort(List<JointType> numjoint) 
    {

        JointType temp;
        for(int j = 0; j <= numjoint.Count -2; j++)
        {
            for(int i =0; i <= numjoint.Count -2; i++)
            {
                if(numjoint[i] > numjoint[i+1])
                {
                    temp = numjoint[i + 1];
                    numjoint[i + 1] = numjoint[i];
                    numjoint[i] = temp;
                }
            }
        }
        foreach(JointType p  in numjoint)
        {
            Debug.Log(p + " ");
        }
        
        return numjoint;
    }
    #endregion

    #region calculo de pares de vectores (dados joints y coordenadas en 5 instantes) 
    public List<Vector3> CalculoVectores(Dictionary<JointType, List<Vector3>> dict)
    {
        //calculo de los vectores directores con la lectura del diccionario resultante de la lectura del archivo 

        List<JointType> listNumJoints = new List<JointType>();

        //Obtiene los keys(numero correspondiente de cada joint) para ordenarlo con 'bubble sort'
        foreach (KeyValuePair<JointType, List<Vector3>> coordenaLeida in dict)
        {
            listNumJoints.Add(coordenaLeida.Key);
        }
        listNumJoints = bubbleSort(listNumJoints); //ORDENA LA LISTA POR JOINTS CRECIENTE

        //Calculo de pares de vectores (Hombro-Codo, Codo-Mano)
        Vector3[,] v = new Vector3[listNumJoints.Count -1 ,dict[listNumJoints[0]].Count];
        for (int i = 0; i < listNumJoints.Count -1; i++)
        {
            for (int j = 0; j < dict[listNumJoints[i]].Count  ;j++)
            {
                v[i, j].x = dict[listNumJoints[i]][j].x - dict[listNumJoints[i + 1]][j].x;
                v[i, j].y = dict[listNumJoints[i]][j].y - dict[listNumJoints[i + 1]][j].y;
                v[i, j].z = dict[listNumJoints[i]][j].z - dict[listNumJoints[i + 1]][j].z;
            }   
        }
        
        List<Vector3> currentPosition = new List<Vector3>();
        for(int i=0; i < listNumJoints.Count; i++)
        {
            currentPosition.Add(GetVector3FromJoint(currentJoint[listNumJoints[i]]));
        }

        //// calcular distancia entre joints 'en directo' (modulo de un vector)
        List<float> listaDistancia = new List<float>();
        for (int i = 0; i < currentPosition.Count - 1; i++)
        {
            float distancia = Vector3.Distance(currentPosition[i], currentPosition[i + 1]);
            listaDistancia.Add(distancia);
        }

        List<Vector3> listaParesV = new List<Vector3>();
        List<Vector3> posicionFinalObjeto = new List<Vector3>();

        for (int i = 0; i < v.GetLength(1) ; i++) //Acceso a cada columna de v[0,1]
        {
            for (int j = 0; j < v.GetLength(0); j++)//Acceso a cada fila
            {
                listaParesV.Add(v[j, i]); //Anyado fila (j) y columna (i)
            }
            Debug.Log("ListaParesVectores: " + listaParesV.Count + ", ListaDistancia: " + listaDistancia.Count);
            posicionFinalObjeto.Add(calculoPosiciones(listaParesV, currentPosition[0], listaDistancia));

            listaParesV.Clear();
        }

        BubbleManager.setListaPos(posicionFinalObjeto);
        return posicionFinalObjeto;
    }
    #endregion

    //public Vector3 GetPositionFroList(List<Vector3> lista)
    //{
    //    foreach (Vector3 pos in lista)
    //        return new Vector3(pos.x, pos.y, 0);
    //}

    #region metodo recursivo de calculo de posiciones finales de objeto
    public Vector3 calculoPosiciones(List<Vector3> listaParesVectores, Vector3 posicionLive, List<float> listaDistancia)
    {
        #region explicacion metodo
        // 1_ Vector listaParesVectores (x,y,z)
        //2_ Distancia d dada listaDistancia
        //3_ Sacar vector unitario Vu de listaParesVectores
        //4_ Vector resultante es Vr= Vu * d 
        //5_ la posicion deseada final P = coordenada J1(x,y,z) + Vr
        #endregion
        
        Vector3 vectorUnit= listaParesVectores[0] / listaParesVectores[0].magnitude;

        Vector3 vectUnitDist = vectorUnit * listaDistancia[0];
        Vector3 c = vectUnitDist + posicionLive;
        Vector3 coord;

        if (listaParesVectores.Count > 1 && listaDistancia.Count > 1)
            coord = calculoPosiciones(listaParesVectores.GetRange(1, listaParesVectores.Count-1), c, listaDistancia.GetRange(1, listaDistancia.Count-1));
        else
            coord = c;

        return coord;
    }
    #endregion
}
