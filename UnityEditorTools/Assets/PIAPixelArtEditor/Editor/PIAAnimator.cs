﻿using UnityEngine;

public class PIAAnimator{

    #region Static

    private static PIAAnimator _instance;
    public static PIAAnimator Instance
    {
        get
        {
            if (_instance == null)
                _instance = new PIAAnimator();
            return _instance;
        }
    }

    #endregion

    #region Fields

    private int currentFrameInPreview = 0;
    private float timer = 0;
    PIAImageData imageData;

    #endregion

    #region Properties

    public int Speed { get; set; }

    #endregion

    #region Methods

    public PIAAnimator() {
        Speed = 1;
    }

    public void Update() {

        timer += Time.deltaTime * Speed;
        if (timer >= 1)
        {
            timer = 0;
            currentFrameInPreview = (currentFrameInPreview + 1) % PIASession.Instance.ImageData.Frames.Count;
        }
    }
    public PIAFrame GetFrameOrFirst() {
        PIAFrame output;
        imageData = PIASession.Instance.ImageData;

        if (currentFrameInPreview < imageData.Frames.Count)
            output = imageData.Frames[currentFrameInPreview] == null ? imageData.Frames[0] : imageData.Frames[currentFrameInPreview];
        else
            output = imageData.Frames[0];

        return output;
    }
    #endregion
}
