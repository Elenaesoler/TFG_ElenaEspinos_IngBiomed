using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
//GUIUtility.ExitGUI();

public class MeasureDepth : MonoBehaviour
{
    public MultiSourceManager mMultiSource;
    public Texture2D mDepthTexture;

    private ushort[] mDepthData= null;
    private CameraSpacePoint[] mCameraSpacePoints = null;
    private ColorSpacePoint[] mColorSpacePoints = null;


    private KinectSensor mSensor = null;
    private CoordinateMapper mMapper = null;

    private readonly Vector2Int mDepthResolution = new Vector2Int(512, 424);
    
    private void awake()
    {
        mSensor = KinectSensor.GetDefault();
        mMapper = mSensor.CoordinateMapper;

        int arraySize = mDepthResolution.x * mDepthResolution.y;

        mCameraSpacePoints = new CameraSpacePoint[arraySize];
        mColorSpacePoints = new ColorSpacePoint[arraySize];
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DepthToColor();

            mDepthTexture = CreateTexture();
        }
    }

    private void DepthToColor()
    {
        // Get depth
        mDepthData = mMultiSource.GetDepthData();

        //Mapper
        mMapper.MapDepthFrameToCameraSpace(mDepthData, mCameraSpacePoints);
        mMapper.MapDepthFrameToColorSpace(mDepthData, mColorSpacePoints);

        //filler

    }
    private Texture2D CreateTexture()
    {
        Texture2D newTexture = new Texture2D(1920, 1080, TextureFormat.Alpha8, false);

        for(int x=0; x < 1920; x++)
        {
            for(int y=0; y< 1080; y++)
            {
                newTexture.SetPixel(x, y, Color.clear);
            }
        }
        foreach(ColorSpacePoint point in mColorSpacePoints)
        {
            newTexture.SetPixel((int) point.X, (int) point.Y, Color.black);
        }

        newTexture.Apply();

        return newTexture;

    }
}
