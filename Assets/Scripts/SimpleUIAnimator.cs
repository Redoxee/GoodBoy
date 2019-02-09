using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleUIAnimator : MonoBehaviour
{
    [SerializeField]
    AnimationCurve yAnim = null;

    [SerializeField]
    float delay = 0f;

    [SerializeField]
    float duration = 1f;

    private RectTransform rect;
    private Vector3 savedPos;


    private void Awake()
    {
        this.rect = this.transform as RectTransform;
        this.savedPos = this.rect.localPosition;
    }

    private void Update()
    {
        float progress = ((Time.realtimeSinceStartup + this.delay) % this.duration) / this.duration;
        float dy = this.yAnim.Evaluate(progress);
        Vector3 np = this.savedPos;
        np.y += dy;
        rect.localPosition = np;

    }
}
