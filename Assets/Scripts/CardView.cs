using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [SerializeField]
    private RectTransform contentTransform = null;
    [SerializeField]
    private float smallHeight = 400f;
    [SerializeField]
    private float deployedHeight = 400f;
    [SerializeField]
    private AnimationCurve deployCurve = null;
    [SerializeField]
    private float animationLength = 1f;
    private float animationTime = -1f;
    private CardState currentState = CardState.Folded;

    [SerializeField]
    private UnityEngine.UI.Image voteImage = null;
    [SerializeField]
    private Sprite voteGoodSprite = null;
    [SerializeField]
    private Sprite voteBadSprite = null;
    [SerializeField]
    private AnimationCurve voteAlphaCurve = null;
    [SerializeField]
    private AnimationCurve voteScaleCurve = null;

    private void Update()
    {
        this.UpdateDeployment();
    }

    public CardState CurrentState
    {
        get
        {
            return this.currentState;
        }
        set
        {
            if (value != this.currentState)
            {
                this.currentState = value;
                this.animationTime = this.animationLength;
            }
        }
    }

    private void UpdateDeployment()
    {
        if (animationTime < 0f)
        {
            return;
        }

        this.animationTime -= Time.deltaTime;
        this.animationTime = Mathf.Max(this.animationTime, 0f);
        float progression = this.animationTime / this.animationLength;
        if (this.currentState == CardState.Deployed)
        {
            progression = 1f - progression;
        }
        progression = Mathf.Clamp01(progression);

        this.TweenHeight(progression);
    }

    private void TweenHeight(float p)
    {
        Vector2 size = this.contentTransform.sizeDelta;
        size.y = this.smallHeight + this.deployCurve.Evaluate(p) * (this.deployedHeight - this.smallHeight);
        this.contentTransform.sizeDelta = size;
    }

    public void ForceClose()
    {
        this.currentState = CardState.Folded;
        this.TweenHeight(0f);
        this.animationTime = -1f;
    }

    public enum CardState
    {
        Folded,
        Deployed,
    }

    public void SetVoteState(bool isGood, float progression = 0f)
    {
        Sprite nextSprite = isGood ? this.voteGoodSprite : this.voteBadSprite;
        if (this.voteImage.sprite != nextSprite)
        {
            this.voteImage.sprite = nextSprite;
        }
        progression = Mathf.Clamp01(progression);
        float colProgression = this.voteAlphaCurve.Evaluate(progression);
        Color color = new Color(1, 1, 1, colProgression);
        this.voteImage.color = color;
        float scaleProg = this.voteScaleCurve.Evaluate(progression);
        this.voteImage.transform.localScale = new Vector3(scaleProg, scaleProg, scaleProg);
    }
}
