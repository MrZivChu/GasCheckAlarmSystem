using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbeInfo : MonoBehaviour
{
    public ProbeModel currentModel;
    public void InitInfo(ProbeModel model)
    {
        currentModel = model;
    }
}
