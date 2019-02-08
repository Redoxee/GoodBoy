﻿using System.Collections;
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

    [SerializeField]
    private CardHandler cardHandler = null;

    [SerializeField]
    private Loader ContentLoader = null;
    private ProfileGenerator profileGenerator = null;

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
        
        this.ContentLoader.Load();
        this.profileGenerator = new ProfileGenerator(this.ContentLoader.Database);

        this.cardHandler.OnContentLoaded();
    }

    public void OnQuickLoveClicked()
    {
        this.cardHandler.QuickSelection(true);
    }

    public void OnQuickNoCLicked()
    {
        this.cardHandler.QuickSelection(false);
    }

    public Profile GetNewProfile()
    {
        return this.profileGenerator.GenerateProfile();
    }
}
