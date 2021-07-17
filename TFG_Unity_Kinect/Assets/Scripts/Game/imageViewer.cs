using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;


public class imageViewer : MonoBehaviour
{
    public MeasureDepth mMeasureDepth;
    public MultiSourceManager mMultiSource;

    public RawImage mRawImage; //output color image
    //public RawImage mRawDepth;

    void Update()
    {
        mRawImage.texture = mMultiSource.GetColorTexture();

        //mRawDepth.texture = mMeasureDepth.mDepthTexture;


    }
}
