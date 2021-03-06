﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private UnityEngine.UI.Image voteYesImage = null;
    [SerializeField]
    private UnityEngine.UI.Image voteNoImage = null;
    [SerializeField]
    private Sprite voteGoodSprite = null;
    [SerializeField]
    private Sprite voteBadSprite = null;
    [SerializeField]
    private AnimationCurve voteAlphaCurve = null;
    [SerializeField]
    private AnimationCurve voteScaleCurve = null;
    [SerializeField]
    private UnityEngine.UI.Image backgroundVote = null;
    [SerializeField]
    [Range(0, 1)]
    private float barckgroundVoteAlphaFactor = .5f;
    [SerializeField]
    private ProfileView profileView;

    public Profile Profile
    {
        private set;
        get;
    }

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
        Image voteImage = isGood ? this.voteYesImage : this.voteNoImage;
        Image notVoteImage = !isGood ? this.voteYesImage : this.voteNoImage;

        notVoteImage.gameObject.SetActive(false);
        voteImage.gameObject.SetActive(true);

        progression = Mathf.Clamp01(progression);
        float colProgression = this.voteAlphaCurve.Evaluate(progression);
        Color color = new Color(1, 1, 1, colProgression);
        voteImage.color = color;
        float scaleProg = this.voteScaleCurve.Evaluate(progression);
        voteImage.transform.localScale = new Vector3(scaleProg, scaleProg, scaleProg);

        color = Color.white; // isGood ? Color.white : Color.black;
        color.a = colProgression * this.barckgroundVoteAlphaFactor;
        this.backgroundVote.color = color;
    }

    public void Setup(Profile profile)
    {
        this.Profile = profile;

        this.profileView.ProfilePic.sprite = profile.Pict;
        this.profileView.Name.text = profile.Name;

        string tags = string.Empty;
        for (int index = 0; index < profile.Tags.Length; ++index)
        {
            tags += profile.Tags[index] + " ";
        }
        this.profileView.Tags.text = tags;

        string trivias = string.Empty;
        for (int index = 0; index < profile.Trivias.Length; ++index)
        {
            trivias += profile.Trivias[index];
            if (index + 1 < profile.Trivias.Length)
            {
                trivias += "\n";
            }
        }
        this.profileView.Trivias.text = trivias;

        LayoutRebuilder.ForceRebuildLayoutImmediate(this.contentTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.contentTransform);
    }

    [System.Serializable]
    public struct ProfileView
    {
        public Image ProfilePic;
        public TMPro.TMP_Text Name;
        public TMPro.TMP_Text Tags;
        public TMPro.TMP_Text Trivias;
    }
}
