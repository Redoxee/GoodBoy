using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProcess : MonoBehaviour
{
    private static GameProcess InstanceStatic = null;

    public static GameProcess Instance
    {
        get
        {
            return GameProcess.InstanceStatic;
        }
    }

    [SerializeField]
    private SlideManager slideManager = null;
    public SlideManager SlideManager
    {
        get
        {
            return this.slideManager;
        }
    }

    private void Awake()
    {
        if (GameProcess.InstanceStatic != null)
        {
            if (GameProcess.InstanceStatic == this)
            {
                return;
            }

            Debug.LogError("Multiple instance of GameProcess;");
            Destroy(this);
            return;
        }

        GameProcess.InstanceStatic = this;
    }
}
