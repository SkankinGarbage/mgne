using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "FadeIndexData", menuName = "Data/Index/FadeIndexData")]
public class FadeIndexData : GenericIndex<FadeData> {

}

[Serializable]
public class FadeData : GenericDataObject {

    [Range(-1, 1)] public float brightnessMod;
    public float delay;
    public bool invert;

    // copy constructor
    public FadeData(FadeData source) {
        brightnessMod = source.brightnessMod;
        delay = source.delay;
        invert = source.invert;
    }
}
