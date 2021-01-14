using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;


public class imageViewer : MonoBehaviour
{
    public MultiSourceManager mMultiSource;

    public RawImage mRawImage; //output color image

    void Update()
    {
        mRawImage.texture = mMultiSource.GetColorTexture();

    }
}
