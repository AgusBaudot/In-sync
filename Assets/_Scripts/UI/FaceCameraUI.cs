using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCameraUI : MonoBehaviour
{
    private void LateUpdate()
    {
        //Vector3 cameraPos = Helpers.Camera.transform.position;
        //cameraPos.y = transform.position.y;
        //transform.LookAt(cameraPos);
        //transform.Rotate(0, 180, 0);

        transform.rotation = Quaternion.LookRotation(transform.position - Helpers.Camera.transform.position);
    }
}
