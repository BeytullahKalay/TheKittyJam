using Pandoras.Helper;
using UnityEngine;

public class CanvasLookAtCamera : MonoBehaviour
{
    private Camera _mainCam;

    private void Awake()
    {
        _mainCam = Camera.main;
    }

    private void Update()
    {

        var dir = (transform.position - _mainCam.transform.position).normalized;
        var angle = Helpers.GetAngleFromVectorFloatForRotZ(dir);

        transform.rotation = Quaternion.Euler(new Vector3(angle, 180, 0));
    }
}
