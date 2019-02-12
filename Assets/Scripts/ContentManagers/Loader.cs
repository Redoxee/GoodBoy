using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Loader : MonoBehaviour
{
    public TextAsset[] FileToLoad = null;
    
    public ContentDatabase Database;
    [System.NonSerialized]
    public bool isLoaded = false;

    public System.Collections.Generic.List<Sprite> Pics = null;

    public abstract bool Load();

}
