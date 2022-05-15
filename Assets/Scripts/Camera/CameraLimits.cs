using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class CameraLimits : CinemachineExtension
{
    public GameObject boundingRect;
    public Camera c;
    public Transform bg;

    private float min_X, max_X, min_Y, max_Y;
    private float cam_W, cam_H;

    void Start() {
        cam_H = c.orthographicSize;
        cam_W = c.orthographicSize * c.aspect;

        Bounds b = boundingRect.GetComponent<Renderer>().bounds;
        min_X = b.min.x + cam_W;
        min_Y = b.min.y + cam_H;
        max_X = b.max.x - cam_W;
        max_Y = b.max.y - cam_H;
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Finalize)
        {
            //camera position
            var pos = state.RawPosition;
            pos.x = Mathf.Clamp(pos.x, min_X, max_X);
            pos.y = Mathf.Clamp(pos.y, min_Y, max_Y);
            state.RawPosition = pos;

            //background scrolling
            float xPerc, yPerc;
            xPerc = (pos.x - min_X) / (max_X - min_X);
            yPerc = (pos.y - min_Y) / (max_Y - min_Y);
            bg.transform.localPosition = new Vector3((float)(2.9f - (5.9 * xPerc)), (float)(5.95 - (11.9 * yPerc)), 10f);
        }
    }
}
