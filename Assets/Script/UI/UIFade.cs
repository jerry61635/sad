using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class UIFade : NetworkBehaviour
{
    public GameObject chatPanel, textObject;
    public InputField chatBox;
    public int maxMessages = 25;
    public float UIHideTime = 0;
    [SerializeField]
    List<Message> messageList = new List<Message>();

    public static bool chatFocus;


    void Awake()
    {
        HideUI();
        chatPanel = GameManager.Instance.chatPanel;
        textObject = GameManager.Instance.textObject;
        chatBox = GameManager.Instance.chatBox;
    }

    private void Update()
    {
        if (chatBox.isFocused) chatFocus = true;
        else chatFocus = false;

        if(Player_Movement.instance.pause || chatFocus) UIHideTime = 3;

        if (chatBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return) && IsClient)
            {
                if (IsServer) SendMessageToChatClientRpc(Player_Movement.instance.name_p + ": " + chatBox.text);
                else SendMessageToChatServerRpc(Player_Movement.instance.name_p + ": " + chatBox.text);
                chatBox.text = "";
            }
        }
        else
        {
            if (!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                chatBox.ActivateInputField();
                UIHideTime = 3;
            }
        }

        if (!chatBox.isFocused)
        {
            //annoucement....
        }

        if (UIHideTime <= 1)
        {
            GameManager.Instance.chatCanvasGroup.alpha -= Time.deltaTime;
        }
        else if (UIHideTime > 1)
        {
            ShowUI();
            UIHideTime -= Time.deltaTime;
        }
        else if (UIHideTime <= 0) return;
    }
    public void ShowUI()
    {
        GameManager.Instance.chatCanvasGroup.alpha = 1;
    }

    public void HideUI()
    {
        GameManager.Instance.chatCanvasGroup.alpha = 0;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendMessageToChatServerRpc(string text)
    {
        if (!IsServer) return;

        SendMessageToChatClientRpc(text);
    }


    [ClientRpc]
    public void SendMessageToChatClientRpc(string text)
    {
        UIHideTime = 3;
        if (messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }
        Message newMessage = new Message();

        newMessage.text = text;
        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObject = newText.GetComponent<Text>();
        newMessage.textObject.text = newMessage.text;
        messageList.Add(newMessage);
    }
}

[System.Serializable]
public class Message
{
    public string text;
    public Text textObject;
}

