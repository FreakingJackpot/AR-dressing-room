using SimpleJSON;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
public class RequestManager : MonoBehaviour
{
    Socket socket;
    IPAddress iP;
    IPEndPoint iPEnd;
    Thread thread;

    bool changePic;

    public byte[] ImageSize;
    public int Len;

    public byte[] Image;

    byte[] Transform;
    JSONNode data;

    public int posePreset = 0;
    public int rotationPreset = 0;

    public GameObject canvas;
    public RawImage rawImage;
    public Transform ring;

    public Text pose;
    public Text rot;

    string a = "Текущая позиция: ";
    string b = "Текущая ротация: ";

    void InitSocket()
    {
        iP = IPAddress.Parse("127.0.0.1");
        iPEnd = new IPEndPoint(iP, 9000);

        thread = new Thread(new ThreadStart(SocketReceive));
        thread.Start();
    }

    void SocketConnect()
    {
        if (socket != null)
            socket.Close();

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(iPEnd);
    }

    void SocketReceive()
    {
        SocketConnect();

        while (true)
        {
            try
            {
                ImageSize = new byte[1024];
                Len = socket.Receive(ImageSize);
                if (Len == 0)
                {
                    SocketConnect();
                    continue;
                }
                else
                {
                    int recv_len = Convert.ToInt32(Encoding.ASCII.GetString(ImageSize, 0, Len));
                    Image = new byte[recv_len];

                    int img_len = socket.Receive(Image);
                    if (recv_len != img_len)
                    {
                        Debug.Log("Битый кадр был отправлен");
                    }
                    else
                    {
                        changePic = true;
                        Transform = new byte[4096];
                        socket.Receive(Transform);

                        string bitString = System.Text.Encoding.UTF8.GetString(Transform);
                        data = JSON.Parse(bitString);
                    }
                }
            }
            catch
            {
                continue;
            }
        }
    }

    private void SocketClose()
    {
        if (thread != null)
        {
            thread.Interrupt();
            thread.Abort();
        }
        if (socket != null)
            socket.Close();
        Debug.Log("Отключение от сервера");
    }

    private void Start()
    {
        rawImage.texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        pose.text = a + posePreset.ToString();
        rot.text = b + rotationPreset.ToString();
        InitSocket();
    }

    void Update()
    {
        if (changePic)
        {
            var texture = rawImage.texture as Texture2D;
            texture.LoadImage(Image);
            texture.Apply();
            SetRingTransform(data);
            changePic = false;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            posePreset++;
            pose.text = a + posePreset.ToString();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            posePreset--;
            pose.text = a + posePreset.ToString();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {

            rotationPreset++;
            rot.text = b + rotationPreset.ToString();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rotationPreset--;
            rot.text = b + rotationPreset.ToString();
        }
    }

    void SetRingTransform(JSONNode json)
    {

        var pos = json["positions"][posePreset];
        var rot = json["rotations"][rotationPreset];
        var rect = canvas.GetComponent<RectTransform>().rect;
        ring.localPosition = new Vector3(pos[0] * rect.width / 2, pos[1] * rect.height / 2, 0);
        ring.localRotation = new Quaternion(rot[0], rot[1], rot[2], rot[3]);
    }

    public void Quit()
    {
        SocketClose();
        Application.Quit();
    }
}
