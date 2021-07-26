using UnityEngine;

public class CamAndroid : MonoBehaviour
{
    [SerializeField] SpriteRenderer backgound;
    [SerializeField] Camera cam;
    void Start()
    {
        float orthographic;
        switch (Screen.height)
        {
            case 800:
                orthographic = 8.91f;
                break;
            case 1280:
                orthographic = 8.98f;
                break;
            case 1920:
                orthographic = 8.98f;
                break;
            case 2160:
                orthographic = 10.11f;
                break;
            case 2960:
                orthographic = 10.41f;
                break;
            default:
                orthographic = 8.98f;
                break;
        }

        cam.orthographicSize = orthographic;
    }

}
