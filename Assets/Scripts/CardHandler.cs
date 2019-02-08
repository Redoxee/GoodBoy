using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHandler : MonoBehaviour, SlideManager.IDragListener
{
    [SerializeField]
    private Camera sceneCamera = null;

    [SerializeField]
    private CardView card1 = null;
    [SerializeField]
    private CardView card2 = null;

    private CardView currentCard = null;
    [SerializeField]
    private float maxVelocity = 10000;
    [SerializeField]
    private float transitionDampFactor = .2f;
    [SerializeField]
    private float holdDampFactor = .1f;
    [SerializeField]
    private float outDampFactor = .1f;
    [SerializeField]
    private float verticalFactor = .3f;
    [SerializeField]
    private float rotationSpeed = 4.0f;


    private float outerBound;
    [SerializeField]
    private float choiceDistanceThreshold = 150f;

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

        this.outerBound = (this.sceneCamera.orthographicSize ) *( (float)Screen.height / (float)Screen.width);
        this.currentCard = card1;
        this.card1.SetVoteState(true, 0);
        this.card2.SetVoteState(true, 0);
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
        Vector3 cardPosition = this.currentCard.transform.position;
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

        if (currentPosX >= this.choiceDistanceThreshold)
        {
            this.TransitionOut(true);
        }
        else if (currentPosX <= -this.choiceDistanceThreshold)
        {
            this.TransitionOut(false);
        }
        else
        {
            this.target = this.forground;
        }
    }

    private void TransitionOut(bool isGood)
    {
        const float margin = 3f;
        float direction = isGood ? 1 : -1;
        this.target = new Vector3(this.outerBound * margin * direction, this.target.y, this.target.z);
        this.isTransitioningOut = true;
    }

    void SlideManager.IDragListener.OnClick(Vector2 pos)
    {
        if (this.isDragging)
        {
            return;
        }
        if (this.target != this.forground)
        {
            return;
        }

        CardView.CardState nextState = this.currentCard.CurrentState == CardView.CardState.Deployed ? CardView.CardState.Folded : CardView.CardState.Deployed;

        this.currentCard.CurrentState = nextState;
    }

    private void Update()
    {
        Vector3 pos = this.currentCard.transform.position;

        if ((pos - this.target).sqrMagnitude < .01)
        {
            return;
        }

        float factor = this.transitionDampFactor;
        if (this.isDragging)
        {
            factor = this.holdDampFactor;
        }
        if (this.isTransitioningOut)
        {
            factor = this.outDampFactor;
        }

        pos = Vector3.SmoothDamp(pos, this.target, ref this.currentVelocity, factor, this.maxVelocity);
        this.currentCard.transform.position = pos;
        float angle = pos.x * this.rotationSpeed;
        this.currentCard.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.back);

        float progress = Mathf.Abs(pos.x) / this.choiceDistanceThreshold;
        bool isGood = pos.x > 0f;
        this.currentCard.SetVoteState(isGood, progress);

        if (this.isTransitioningOut && Mathf.Abs(pos.x) > this.outerBound)
        {
            this.isTransitioningOut = false;
            this.target = this.forground;
            this.currentVelocity = new Vector3(0, 0, 0);
            this.CycleCard();
        }
    }

    private void CycleCard()
    {
        this.currentCard.transform.localRotation = Quaternion.identity;
        this.currentCard.transform.position = this.background;
        this.currentCard.SetVoteState(true, 0);
        this.currentCard.ForceClose();

        Profile profile = GameProcess.Instance.GetNewProfile();
        this.currentCard.Setup(profile);

        if (this.currentCard == this.card2)
        {
            this.currentCard = this.card1;
        }
        else
        {
            this.currentCard = this.card2;
        }

        this.currentCard.transform.position = this.forground;
    }

    public bool IsCurrentCard(Transform transform)
    {
        return this.currentCard == transform;
    }

    public void QuickSelection(bool isGood)
    {
        this.TransitionOut(isGood);
    }

    public void OnContentLoaded()
    {
        Profile prof = GameProcess.Instance.GetNewProfile();
        this.card1.Setup(prof);
        prof = GameProcess.Instance.GetNewProfile();
        this.card2.Setup(prof);
    }
}
