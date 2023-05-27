using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdataColor : MonoBehaviour
{
    private Image self;

    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Image>();
        self.SetMaterialDirty();

    }

    private void LateUpdate()
    {
        self.SetMaterialDirty();
    }
    //private void Update()
    //{
    //    self.SetMaterialDirty();
    //}
}
