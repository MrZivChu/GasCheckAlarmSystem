using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectProbeForGraphItem : MonoBehaviour
{
    public Text probeNameText;
    public Text machineNameText;

    public ProbeModel probeModel;
    public void InitData(ProbeModel probeModel)
    {
        this.probeModel = probeModel;
        probeNameText.text = probeModel.ProbeName;
        machineNameText.text = probeModel.MachineName;
    }
}
