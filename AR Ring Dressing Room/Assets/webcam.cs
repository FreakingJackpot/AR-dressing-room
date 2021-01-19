using UnityEngine;
using UnityEngine.UI;
public class webcam : MonoBehaviour
{
    public WebCamTexture camTexture;
    public RawImage Raw;
    public AspectRatioFitter fit;
    bool available;
    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            available = false;
            return;
        }
        camTexture = new WebCamTexture(devices[0].name, Screen.width, Screen.height);
        camTexture.requestedFPS = 60;
        camTexture.Play();
        Raw.material.mainTexture = camTexture;

        available = true;
    }

    private void Update()
    {
        Resources.UnloadUnusedAssets();
        if (available)
            return;
        float ratio = (float)camTexture.width / (float)camTexture.height;
        fit.aspectRatio = ratio;
        float scaleY = camTexture.videoVerticallyMirrored ? -1f : 1f;
        Raw.rectTransform.localScale = new Vector3(1f, scaleY, 1f);
        int orient = -camTexture.videoRotationAngle;
        Raw.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }
}
