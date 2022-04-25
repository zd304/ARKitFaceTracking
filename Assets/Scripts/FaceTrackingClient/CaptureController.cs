using UnityEngine;
using UnityEngine.UI;

public class CaptureController : MonoBehaviour
{
    public GameObject connectPanel;
    public GameObject connectedPanel;

    public InputField ipInput;
    public InputField portInput;
    public Text connectedInfo;
    public Text fpsInput;

    public Slider thresholdSlider;
    public Text thresholdValue;

    public FaceTrackingClinet client;

    private float fpsCalcTime = 1.0f;
    private int frames = 0;

    private void Awake()
    {
        OnSliderChange();
    }

    public void OnClickConnect()
    {
        if (string.IsNullOrEmpty(ipInput.text) || string.IsNullOrEmpty(portInput.text))
        {
            return;
        }
        int port = 0;
        if (!int.TryParse(portInput.text, out port))
        {
            return;
        }
        client.ip = portInput.text;
        client.port = port;
        client.Connect(ipInput.text, port);
    }

    public void OnSliderChange()
    {
        CaptureClient.coefficientThreshold = thresholdSlider.value;
        thresholdValue.text = thresholdSlider.value.ToString();
    }

    private void Update()
    {
        if (fpsCalcTime > 0.0f)
        {
            fpsCalcTime -= Time.deltaTime;
            ++frames;
            if (fpsCalcTime <= 0.0f)
            {
                fpsInput.text = "FPS:" + frames;
                frames = 0;
                fpsCalcTime = 1.0f;
            }
        }
        if (client != null)
        {
            if (client.IsConnect && !connectedPanel.activeSelf)
            {
                connectedPanel.SetActive(true);
                connectedInfo.text = ipInput.text + ":" + portInput.text;
            }
            if (!client.IsConnect && connectedPanel.activeSelf)
            {
                connectedPanel.SetActive(false);
            }
            if (client.IsConnect && connectPanel.activeSelf)
            {
                connectPanel.SetActive(false);
            }
            if (!client.IsConnect && !connectPanel.activeSelf)
            {
                connectPanel.SetActive(true);
            }
        }
    }
}