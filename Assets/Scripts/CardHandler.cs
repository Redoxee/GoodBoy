using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHandler : MonoBehaviour, SlideManager.IDragListener
{
    [SerializeField]
    private Camera sceneCamera = null;

    [SerializeField]
    private Transform card1 = null;
    [SerializeField]
    private Transform card2 = null;

    private Transform currentCard = null;
    [SerializeField]
    private float maxVelocity = 10000;
    [SerializeField]
    private float transitionDampFactor = .2f;
    [SerializeField]
    private float holdDampFactor = .1f;
    [SerializeField]
    private float verticalFactor = .3f;
    [SerializeField]
    private float rotationSpeed = 4.0f;
    [SerializeField]
    private float outDampFactor = .05f;


    private float outerBound;
    private float choiceDistanceThreshold = 1.5f;

    private Vector3 target = new Vector3(0,0,0);
    private Vector3 currentVelocity = new Vector3(0, 0, 0);

    private bool isDragging;
    private Vector2 startPosition = new Vector2(0, 0);
    private bool isTransitioningOut = false;


    private Vector3 forground = new Vector3(0,0,0);
    private Vector3 background = new Vector3(0, 0, 10);

    IEnumerator Start()
    {
        while (GameProcess.Instance == null)
        {
            Debug.Log("Waiting");
            yield return new WaitForFixedUpdate();
        }
        GameProcess.Instance.SlideManager.RegisterListener(this);

        this.outerBound = this.sceneCamera.orthographicSize / (float)Screen.height * (float)Screen.width;

        this.currentCard = card1;
    }

    void SlideManager.IDragListener.OnBeginDrag(Vector2 pos)
    {
        if (!this.isTransitioningOut)
        {
            this.isDragging = true;
            this.startPosition = pos;
        }
    }

    void SlideManager.IDragListener.OnDrag(Vector2 pos)
    {
        if (!this.isDragging)
        {
            return;
        }

        Vector2 delta = pos - this.startPosition;
        Vector3 cardPosition = this.currentCard.position;
        cardPosition.x = delta.x;
        cardPosition.y = delta.y * this.verticalFactor;

        this.target = cardPosition;
    }

    void SlideManager.IDragListener.OnEndDrag(Vector2 pos)
    {
        if (!this.isDragging)
        {
            return;
        }

        this.isDragging = false;

        float currentPosX = this.target.x;

        const float margin = 3f;

        Vector3 nextTarget;
        if (currentPosX >= this.choiceDistanceThreshold)
        {
            nextTarget = new Vector3(this.outerBound + margin, this.target.y, this.target.z);
            this.isTransitioningOut = true;
        }
        else if (currentPosX <= -this.choiceDistanceThreshold)
        {
            nextTarget = new Vector3(- this.outerBound - margin, this.target.y, this.target.z);
            this.isTransitioningOut = true;
        }
        else
        {
            nextTarget = this.forground;
        }

        this.target = nextTarget;
    }

    private void Update()
    {
        Vector3 pos = this.currentCard.transform.position;
        float factor = this.transitionDampFactor;
        if (this.isDragging)
        {
            factor = this.holdDampFactor;
        }
        if (this.isTransitioningOut)
        {
            factor = this.transitionDampFactor;
        }

        pos = Vector3.SmoothDamp(pos, this.target, ref this.currentVelocity, factor, this.maxVelocity);
        this.currentCard.position = pos;
        float angle = pos.x * this.rotationSpeed;
        this.currentCard.localRotation = Quaternion.AngleAxis(angle, Vector3.back);

        if (this.isTransitioningOut && Mathf.Abs(pos.x) - this.outerBound > 2.5)
        {
            this.isTransitioningOut = false;
            this.target = this.forground;
            this.currentVelocity = new Vector3(0, 0, 0);
            this.CycleCard();
        }
    }

    private void CycleCard()
    {
        this.currentCard.localRotation = Quaternion.identity;
        this.currentCard.position = this.background;
        if (this.currentCard == this.card2)
        {
            this.currentCard = this.card1;
        }
        else
        {
            this.currentCard = this.card2;
        }

        this.currentCard.position = this.forground;
    }
}
