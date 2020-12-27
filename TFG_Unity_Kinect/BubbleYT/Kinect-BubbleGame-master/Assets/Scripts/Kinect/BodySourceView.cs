using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

using Windows.Kinect;
using Joint = Windows.Kinect.Joint;

public class BodySourceView : MonoBehaviour
{
    public BodySourceManager mBodySourceManager;
    public GameObject mJointObject;

    private List<Vector3> coordenadaMano = new List<Vector3>();  //esta lista almacena la posicion de la mano

    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();
    private List<JointType> _joints = new List<JointType>
    {
        //JointType.HandLeft,
        //JointType.HandRight,
        JointType.Head,             //añadido
        //JointType.ShoulderRight,  //añadido
        //JointType.ShoulderLeft,   //añadido

    };

    void Update()
    {
        Debug.Log("Cogiendo datos Kinect");
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
        //Debug.Log(coordenadaMano.Count);
        float dist = Vector3.Distance(targetPosition, coordenadaMano[coordenadaMano.Count - 1]);
        if (dist <= 10)
            coordenadaMano.Add(targetPosition);

        Debug.Log("Dist"+ dist);

        //Debug.Log(targetPosition); //imprime en consola las coordenadas de los joints que tengo en _joints
       // }
    }


    private Vector3 GetVector3FromJoint(Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
