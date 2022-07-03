using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    public class Empty4Raycast : MaskableGraphic
    {
        protected Empty4Raycast()
        {
#if UNITY_5
            useLegacyMeshGeneration = false;
#endif
        }

#if UNITY_4_7
        protected override void OnFillVBO(List<UIVertex> toFill)
#else 
        protected override void OnPopulateMesh(VertexHelper toFill)
#endif
        {
            toFill.Clear();
        }
    }
}