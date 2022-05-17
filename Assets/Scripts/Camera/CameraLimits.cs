using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class CameraLimits : CinemachineExtension
{
    public GameObject boundingRect;
    public bool renderBg;
    public Transform bgs;

    public Transform[] bg_list;

    private float min_X, max_X, min_Y, max_Y;
    private Camera c;
    private float cam_W, cam_H;

    void Start() {
        c = Camera.main;
        cam_H = c.orthographicSize;
        cam_W = c.orthographicSize * c.aspect;

        setBackground(boundingRect, 0);
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Finalize)
        {
            //camera position
            if (boundingRect != null) {
                var pos = state.RawPosition;
                pos.x = Mathf.Clamp(pos.x, min_X, max_X);
                pos.y = Mathf.Clamp(pos.y, min_Y, max_Y);
                state.RawPosition = pos;

                //background scrolling
                if (bgs != null && renderBg) {
                    float xPerc, yPerc;
                    xPerc = (pos.x - min_X) / (max_X - min_X);
                    yPerc = (pos.y - min_Y) / (max_Y - min_Y);
                    //hardcoded values since all backgrounds are the same size
                    Vector3 p = new Vector3((float)(2.9f - (5.9 * xPerc)), (float)(5.95 - (11.9 * yPerc)), 10f);
                    if (!float.IsNaN(p.x) && !float.IsNaN(p.y)) {
                        bgs.transform.localPosition = p;
                    }
                }
            }
        }
    }

    public GameObject getBackground() {
        return boundingRect;
    }

    public void setBackground(GameObject bound, int bg_ind) {
        boundingRect = bound;
        Bounds b = boundingRect.GetComponent<Renderer>().bounds;
        min_X = b.min.x + cam_W;
        min_Y = b.min.y + cam_H;
        max_X = b.max.x - cam_W;
        max_Y = b.max.y - cam_H;
        updateBackgroundImage(bg_ind - 1);
    }

    public void updateBackgroundImage(int bg_ind) {
        for (int i=0; i<bg_list.Length; i++) {
            bg_list[i].gameObject.SetActive(false);
        }
        if (bg_ind >= 0 && bg_ind < bg_list.Length) {
            bg_list[bg_ind].gameObject.SetActive(true);
        }
    }
}
