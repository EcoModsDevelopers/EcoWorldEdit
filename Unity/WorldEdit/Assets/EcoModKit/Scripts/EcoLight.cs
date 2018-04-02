using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AdjustTransformToCurve))]
public class EcoLight : MonoBehaviour
{
    static List<EcoLight> ecoLights = new List<EcoLight>();

    public static void OnGIChanged(bool giEnabled)
    {
        foreach (var light in ecoLights)
            light.gameObject.SetActive(!giEnabled);
    }

    private void Start()
    {
        ecoLights.Add(this);
#if ECO_DEV
        this.gameObject.SetActive(!Settings.GetGraphicsSetting("Graphics/Global Illumination"));
#endif
    }

    private void OnDestroy()
    {
        ecoLights.Remove(this);
    }
}
