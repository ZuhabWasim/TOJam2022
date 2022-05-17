using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomChanging : MonoBehaviour
{
    public GameObject[] boundingRects;
    public Transform player;

    private int br_ind;
    private Vector3[] br_min;
    private Vector3[] br_max;
    private CameraLimits cl;
    
    // Start is called before the first frame update
    void Start()
    {
        br_min = new Vector3[boundingRects.Length];
        br_max = new Vector3[boundingRects.Length];
        cl = GetComponent<CameraLimits>();

        for (int i=0; i < boundingRects.Length; i++) {
            updateBoundingRect(i);
        }
        br_ind = 0;
    }

    void updateBoundingRect(int i) {
        Bounds b = boundingRects[i].GetComponent<Renderer>().bounds;
        br_min[i] = b.min;
        br_max[i] = b.max;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInsideRect(br_ind)) {
            for (int i=0; i< boundingRects.Length; i++) {
                if (i != br_ind) {
                    if (isInsideRect(i)) {
                        br_ind = i;
                        cl.setBackground(boundingRects[i], i);
                        break;
                    }
                }
            }
        }
    }

    bool isInsideRect(int i) {
        if (player.position.x >= br_min[i].x
        && player.position.y >= br_min[i].y
        && player.position.x < br_max[i].x
        && player.position.y < br_max[i].y) {
            return true;
        } else {
            return false;
        }
    }


}
