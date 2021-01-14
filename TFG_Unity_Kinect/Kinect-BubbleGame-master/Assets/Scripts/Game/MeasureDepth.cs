using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
//GUIUtility.ExitGUI();

public class MeasureDepth : MonoBehaviour
{
    public MultiSourceManager mMultiSource;

    private CameraSpacePoint[] mCameraSpacePoints = null;
    private ColorSpacePoint[] mcolorSpacePoints = null;


    private KinectSensor mSensor = null;
    private CoordinateMapper mMapper = null;

    private readonly Vector2Int mDepthResolution = new Vector2Int(512, 424);
    
    private void awake()
    {
        mSensor = KinectSensor.GetDefault();
        mMapper = mSensor.CoordinateMapper;

        int arraySize = mDepthResolution.x * mDepthResolution.y;

        mCameraSpacePoints = new CameraSpacePoint[arraySize];
        mcolorSpacePoints = new ColorSpacePoint[arraySize];
    }
}
