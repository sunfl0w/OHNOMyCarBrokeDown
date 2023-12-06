using System.Collections.Generic;
using System.Linq;
//using UnityEditor.Media;
using UnityEngine;
using UnityEngine.Rendering;

public class PPFXManager : MonoBehaviour
{
    public Volume volume;
    public List<VolumeProfile> vplist = new List<VolumeProfile>();

    private static PPFXManager instance;
    public static PPFXManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        ItemInspectGUI.onInspectionGUIEnter += SetBlurProfile;
        ItemInspectGUI.onInspectionGUILeave += SetBaseProfile;

    }

    void SetBlurProfile()
    {
        SetProfile("BlurVP");
    }

    void SetBaseProfile()
    {
        SetProfile("BaseVP");
    }

    void SetProfile(string vpName)
    {
        VolumeProfile vp = vplist.FirstOrDefault(vp => vp.name == vpName);
        if (vplist.Count > 0 && vp != null)
        {
            Debug.Log("New current volume profile is: " + vpName);
            volume.profile = vp;
        }
        else
        {
            Debug.Log("Missing " + vpName + " volume profile");
        }
    }
}