using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;


public class imageViewer : MonoBehaviour
{
    public MultiSourceManager mMultiSource;

    public RawImage mRawImage;

    void Update()
    {
        mRawImage.texture = mMultiSource.GetColorTexture();

    }
}
