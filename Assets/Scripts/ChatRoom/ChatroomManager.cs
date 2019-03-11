using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatroomManager : MonoBehaviour
{
    [SerializeField]
    private GameObject userMessagePrefab = null;
    [SerializeField]
    private GameObject dogMessagePrefab = null;
    [SerializeField]
    private RectTransform messagesRoot = null;
    [SerializeField]
    private InputField inputMessage = null;
    [SerializeField]
    private Text DogNameText = null;

    public List<string> DogSpeech = null;

    public Dictionary<string, List<List<string>>> DogSpecialSpeech = null;

    private List<GameObject> postedMessages = new List<GameObject>();

    private List<string> delayedDogMessage = new List<string>();
    private float currentWaitTime = -1f;

    [SerializeField]
    private float minWaitTime = 1f;
    [SerializeField]
    private float maxWaitTime = 15f;

    [SerializeField]
    private Transform WrittingNotif = null;
    [SerializeField]
    private Image WrittingPict = null;

    private Profile currentProfile = null;

    [SerializeField]
    private float timeBeforeWrittingNotif = 5f;

    public void OnSendPressed()
    {
        this.PostMessage();
    }

    public void OnTextInputValidated()
    {
        this.PostMessage();
    }

    public void OnBackPressed()
    {
        GameProcess.Instance.CloseChatRoom();
    }

    public void Setup(Profile profile, List<string> dogSpeeches, Dictionary<string, List<List<string>>> specialSpeeches)
    {
        this.DogSpeech = dogSpeeches;
        this.DogSpecialSpeech = specialSpeeches;
        if (profile == this.currentProfile)
        {
            return;
        }

        this.delayedDogMessage.Clear();
        for (int index = this.postedMessages.Count - 1; index > -1; --index)
        {
            Destroy(this.postedMessages[index]);
        }

        this.postedMessages.Clear();

        this.currentProfile = profile;
        this.DogNameText.text = profile.Name;
        this.WrittingPict.sprite = profile.Pict;
    }

    private void PostMessage()
    {
        string message = inputMessage.text;
        inputMessage.text = string.Empty;

        if (message.Length < 1)
        {
            return;
        }

        GameObject userMessageObject = UnityEngine.GameObject.Instantiate(this.userMessagePrefab, this.messagesRoot, false);
        Text text = userMessageObject.GetComponent<MessageView>().MessageBody;
        text.text = message;
        
        this.postedMessages.Add(userMessageObject);

        string trigger = message.Trim().ToLower();

        if (this.DogSpecialSpeech.ContainsKey(trigger))
        {
            int index = UnityEngine.Random.Range(0, this.DogSpecialSpeech[trigger].Count);
            foreach (string doggoMessage in this.DogSpecialSpeech[trigger][index])
            {
                string formated = string.Format(doggoMessage, this.currentProfile.Name);
                this.delayedDogMessage.Add(formated);
            }
        }
        else
        {
            int index = UnityEngine.Random.Range(0, this.DogSpeech.Count);
            this.delayedDogMessage.Add(this.DogSpeech[index]);
        }

        if (this.currentWaitTime < 0)
        {
            this.currentWaitTime = UnityEngine.Random.Range(this.minWaitTime, this.maxWaitTime);
        }

        this.RecomputeLayout();
    }

    private void SpawnDogMessage()
    {
        GameObject dogMessage = UnityEngine.GameObject.Instantiate(this.dogMessagePrefab, this.messagesRoot, false);

        string speech = this.delayedDogMessage[0];
        this.delayedDogMessage.RemoveAt(0);
        MessageView messageView = dogMessage.GetComponent<MessageView>();
        messageView.MessageBody.text = speech;
        messageView.Pict.sprite = this.currentProfile.Pict;

        this.WrittingNotif.gameObject.SetActive(false);
        this.RecomputeLayout();

        this.postedMessages.Add(dogMessage);
    }

    private void RecomputeLayout()
    {
        this.WrittingNotif.SetAsLastSibling();

        LayoutRebuilder.ForceRebuildLayoutImmediate(this.messagesRoot);
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.messagesRoot);
    }

    private void Update()
    {
        if (this.delayedDogMessage.Count == 0)
        {
            return;
        }

        if (this.currentWaitTime < 0f)
        {
            return;
        }

        this.currentWaitTime -= Time.deltaTime;

        if (this.currentWaitTime < 0f)
        {
            this.SpawnDogMessage();

            if (this.delayedDogMessage.Count > 0)
            {
                this.currentWaitTime = UnityEngine.Random.Range(this.minWaitTime, this.maxWaitTime) + this.timeBeforeWrittingNotif;
            }
        }

        if (this.delayedDogMessage.Count > 0 && this.currentWaitTime < this.timeBeforeWrittingNotif && !this.WrittingNotif.gameObject.activeSelf)
        {
            this.WrittingNotif.gameObject.SetActive(true);
        }
    }
}
