using SimpleJSON;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RequestManager : MonoBehaviour
{
    private string url = "";
    public RawImage rawImage;
    public int posePreset = 0;
    public int rotationPreset = 0;
    public GameObject canvas;
    public Transform ring;
    WebCamTexture webcamTex;
    Texture2D texture2D;
    public Text pose;
    public Text rot;
    string a = "Текущая позиция: ";
    string b = "Текущая ротация: ";

    public class Data
    {
        public byte[] image;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            posePreset++;
            pose.text = a + posePreset.ToString();
            rot.text = b + rotationPreset.ToString();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            posePreset--;
            pose.text = a + posePreset.ToString();
            rot.text = b + rotationPreset.ToString();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            pose.text = a + posePreset.ToString();
            rot.text = b + rotationPreset.ToString();
            rotationPreset++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rotationPreset--;
            pose.text = a + posePreset.ToString();
            rot.text = b + rotationPreset.ToString();
        }

        webcamTex = (WebCamTexture)rawImage.material.mainTexture;
        texture2D = new Texture2D(webcamTex.width, webcamTex.height);
        texture2D.SetPixels32(webcamTex.GetPixels32());
        Data data = new Data();
        data.image = texture2D.EncodeToJPG();
        string json = JsonUtility.ToJson(data);
        StartCoroutine(PostRequest("http://127.0.0.1:5000/s", json));
    }

    IEnumerator PostRequest(string url, string json)
    {
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else if (uwr.downloadHandler.text != null)
        {
            Debug.Log(uwr.downloadHandler.text);
            var json1 = JSON.Parse(uwr.downloadHandler.text);

            var pos = json1["positions"][posePreset];
            var rot = json1["rotations"][rotationPreset];
            var rect = canvas.GetComponent<RectTransform>().rect;
            ring.localPosition = new Vector3(pos[0] * rect.width / 2, pos[1] * rect.height / 2, 0);
            ring.localRotation = new Quaternion(rot[0], rot[1], rot[2], rot[3]);
        }
    }

    private float Lerp(float x1, float x2, float x3, float y1, float y2)
    {
        return ((x2 - x3) * y1 + (x3 - x1) * y2) / (x2 - x1);
    }

    public void da()
    {
        Application.Quit();
    }
}
