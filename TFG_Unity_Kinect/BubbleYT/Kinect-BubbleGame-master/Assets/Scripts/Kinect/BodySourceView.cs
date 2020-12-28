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

    //https://docs.unity3d.com/ScriptReference/Vector3.Lerp.html
    //public int interpolationFramesCount = 180; //numero de frames para interpolar dos posiciones
    //int elapsedFrames = 0;

    private List<Vector3> coordenadaMano = new List<Vector3>();  //esta lista almacena la posicion de la mano

    //crear los vectores de hombro-mano, hombro-hombro, cadera-cadera y de cadera-hombro
    //con hombro-cadera y hombro-mano sacamos el angulo que forman 
    //con hombro-hombro y cadera-cadera comprobar que se mantienen paralelas o con igual angulo que el inicial


    // https://stackoverflow.com/questions/21219797/how-to-get-correct-timestamp-in-c-sharp
    //timestamp para iniciar la grabacion en un instante y cuando pasen 10seg por ejemplo, que pare de grabar. 
    //que coja la coordenada cada 30 frames por ejemplo
    // que coja t=0 t=0+30 t=0+60 ... y pare a los 10 segundos. con un while(timestamp 

    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();
    private List<JointType> _joints = new List<JointType>
    {
        JointType.HandLeft,
        JointType.HandRight,
        JointType.Head,             //añadido
        //JointType.ShoulderRight,  //añadido
        //JointType.ShoulderLeft,   //añadido

    };

    void Update()
    {
        Debug.Log("Dentro de update() -- BodySourceView");
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

        
    private GameObject CreateBodyObject(ulong id)
    {
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
        //foreach (JointType _joint in _joints)
       // {
            // Get new target position
        Joint sourceJoint = body.Joints[JointType.HandRight]; //_joint por JointType.HandRight
        Vector3 targetPosition = GetVector3FromJoint(sourceJoint);
        targetPosition.z = 0;  // posicion z=0 SIEMPRE para que la pompa y la mano esten en la misma coordenada en 3D

        Debug.Log("Estoy en grabar");
                
        // Get joint, set new position
        Transform jointObject = bodyObject.transform.Find(JointType.HandRight.ToString()); //el cuerpo que reconoce 
        jointObject.position = targetPosition;

        if (coordenadaMano.Count == 0)
        {
            coordenadaMano.Add(targetPosition);
            return;
        }

        #region crear txt con las coordenadas

        //coordenadaMano.ToString();  //las coordenads de tipo vector3 las paso  string para usarlas en el txt
        //TextWriter tw = new StreamWriter("archivoNuevo.txt");
        //tw.WriteLine(List.coordenadaMano);
        
        ////bucle que meta cada coordenadaMano en la variable coordinates
        //Vector3 coordinates = coordinates.Add(coordenadaMano);
        //System.IO.File.WriteAllLines(@"C:\Users\Nena\Desktop\TFG\WriteText.txt", coordinates);
        //Debug.Log(coordinates);
        #endregion

        Debug.Log(coordenadaMano.Count);

        #region distancia entre coordenada actual y coordenada anterior
        float dist = Vector3.Distance(targetPosition, coordenadaMano[coordenadaMano.Count - 1]);
        if (dist <= 10)
        { 
            coordenadaMano.Add(targetPosition);

            //float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;
            //Vector3 interpolatedPosition = Vector3.Lerp(targetPosition, coordenadaMano[coordenadaMano.Count - 1], interpolationRatio);

            //elapsedFrames = (elapsedFrames + 1) % (interpolationFramesCount + 1);  // reset elapsedFrames to zero after it reached (interpolationFramesCount + 1)

            Debug.Log("targetposition "+ targetPosition); 
            Debug.Log("coordenadaMano "+ coordenadaMano);
         

            Debug.Log("Dist"+ dist);
            #endregion
        }
        //Debug.Log(targetPosition); //imprime en consola las coordenadas de los joints que tengo en _joints
        // }
    }
        

    private Vector3 GetVector3FromJoint(Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
