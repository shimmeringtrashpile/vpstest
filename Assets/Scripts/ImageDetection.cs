using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Niantic.ARDK.AR;
using Niantic.ARDK.AR.Configuration;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.Extensions;
using Niantic.ARDK.Utilities.Input.Legacy;
using Niantic.ARDK.AR.HitTest;
using Niantic.ARDK.AR.WayspotAnchors;
using Niantic.ARDK.LocationService;
using Niantic.ARDK.AR.ReferenceImage;
using Niantic.ARDK.Utilities.Collections;
using Grapeshot;
using System;

public class ImageDetection : MonoBehaviour
{

    string jpegFilePath = Application.streamingAssetsPath + "grandcanyon.jpg";

    private IARSession _session;  // A session that is initialized elsewhere.

    [SerializeField]
    public IARWorldTrackingConfiguration worldTrackingConfiguration;



    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RunSessionWithImageDetectionAsynchronously(worldTrackingConfiguration));
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    //public void RunSessionWithImageDetection
    //(IARWorldTrackingConfiguration config)
    //{
    //float imageSize = 0.3f;  // 30 centimeters.

    //var imageSet = new HashSet<IARReferenceImage>();

    //var referenceImage1 = ARReferenceImageFactory.Create("image1", jpegFilePath, imageSize);
    //if (referenceImage1 != null)
    //    imageSet.Add(referenceImage1);

    //byte[] jpegFileContents = new byte[0];  // Load jpeg from a file, or receive the bytes from the network.
    //var referenceImage2 = ARReferenceImageFactory.Create("image2", jpegFileContents, jpegFileContents.Length, imageSize);
    //if (referenceImage2 != null)
    //    imageSet.Add(referenceImage2);

    //// The raw RGBA values of an image, possibly loaded from a file like a PNG, received from the network, or extracted from the camera.
    //// In this case they're in ARGB format.
    //byte[] rawImage = new byte[0];
    //int rawImageWidth = 300;
    //int rawImageHeight = 300;
    //var referenceImage3 = ARReferenceImageFactory.Create("image3", rawImage,
    //    rawImageWidth, rawImageHeight, ByteOrderInfo.big32, AlphaInfo.First, 4, imageSize);
    //if (referenceImage3 != null)
    //    imageSet.Add(referenceImage3);

    //config.DetectionImages = imageSet.AsArdkReadOnly();

    ////session.Run(config);
    //}


    public IEnumerator RunSessionWithImageDetectionAsynchronously
    (IARWorldTrackingConfiguration config)
    {
    float imageSize = 0.3f;  // 30 centimeters.

    var imageSet = new HashSet<IARReferenceImage>();

    var imageProcessed = false;
    Action<IARReferenceImage> addImageToSet = image =>
    {
        if (image != null)
        imageSet.Add(image);

        imageProcessed = true;
    };

    ARReferenceImageFactory.CreateAsync("image1", jpegFilePath, imageSize, addImageToSet);

    while (!imageProcessed)
        yield return null;

    imageProcessed = false;

    byte[]
        jpegFileContents =
        new byte[0]; // Load jpeg from a file, or receive the bytes from the network.

    ARReferenceImageFactory.CreateAsync
        ("image2", jpegFileContents, jpegFileContents.Length, imageSize, addImageToSet);

    while (!imageProcessed)
        yield return null;

    imageProcessed = false;

    // The raw RGBA values of an image, possibly loaded from a file like a PNG, received from the network, or extracted from the camera.
    // In this case they're in ARGB format.
    byte[] rawImage = new byte[0];
    int rawImageWidth = 300;
    int rawImageHeight = 300;
    ARReferenceImageFactory.CreateAsync
    (
        "image3",
        rawImage,
        rawImageWidth,
        rawImageHeight,
        ByteOrderInfo.big32,
        AlphaInfo.First,
        4,
        imageSize,
        addImageToSet
    );

    while (!imageProcessed)
        yield return null;

    imageProcessed = false;

    config.SetDetectionImagesAsync
    (
        imageSet.AsArdkReadOnly(),
        delegate { _session.Run(config); }
    );
    }
}
