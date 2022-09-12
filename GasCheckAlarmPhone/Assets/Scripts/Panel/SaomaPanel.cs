using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class SaomaPanel : MonoBehaviour
{
    public Button cancel;

    private void Start()
    {
        cancel.onClick.AddListener(() =>
        {
            isGoScanning = false;
            webCamTextrue.Stop();
            gameObject.SetActive(false);
        });
    }

    BarcodeReader barcodeReader = null;
    WebCamTexture webCamTextrue = null;
    Action<string, WebCamTexture> callback_ = null;

    public void Run(Action<string, WebCamTexture> callback)
    {
        callback_ = callback;
        WebCamDevice[] devices = WebCamTexture.devices;
        string deviceName = devices[0].name;
        webCamTextrue = new WebCamTexture(deviceName, 600, 600);
        webCamTextrue.Play();
        GetComponent<RawImage>().texture = webCamTextrue;
        barcodeReader = new BarcodeReader();

        interval = 0;
        isGoScanning = true;
        gameObject.SetActive(true);
    }

    bool isGoScanning = false;
    float interval = 0;
    void Update()
    {
        if (isGoScanning)
        {
            interval += Time.deltaTime;
            if (interval >= 0.15f)
            {
                SaoMaRunning();
                interval = 0;
            }
        }
    }

    void SaoMaRunning()
    {
        Color32[] data = webCamTextrue.GetPixels32();
        Result result = barcodeReader.Decode(data, webCamTextrue.width, webCamTextrue.height);
        if (result != null)
        {
            Debug.Log(result.Text);
            isGoScanning = false;
            if (callback_ != null)
            {
                callback_(result.Text, webCamTextrue);
            }
            webCamTextrue.Stop();
            gameObject.SetActive(false);
        }
    }
}
