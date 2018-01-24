using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {
    public float cameraSmoothing;
    public float zDistance;
	void Update () {
        if(PlayerController.player != null) {
            Vector3 pos = PlayerController.GetPlayerPosition();
            pos.z = -zDistance;
            transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * cameraSmoothing);
        }
	}
}
