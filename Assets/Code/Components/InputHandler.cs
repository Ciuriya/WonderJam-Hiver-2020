﻿using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using Boo.Lang;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class InputHandler : MonoBehaviour
{
    public FallingWordSet activeWordSet;
    public FallingWordSet BonusWordSet;
    public TMP_InputField inputField;
    public ScoreHandler ScoreHandler;
    public PhotonView PView;
    public int ValidPlayerID;

    //for animations
    [HideInInspector] public bool typing;

    [Tooltip("Sound played when a valid word is typed")]
    public AudioClip PopSound;
    
    [Tooltip("Sound played when a valid word is typed")]
    public AudioClip KeystrokeSound;

    [Tooltip("Sound played when an invalid word is typed")]
    public AudioClip InvalidSound;

    private AudioSource audioSource;

    public Color highlightColor;

    private void Awake()
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        if(actorNumber != ValidPlayerID) inputField.interactable = false;
        else if(EventSystem.current)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
            GetComponent<Selectable>().OnSelect(null);
        }

        audioSource = GetComponent<AudioSource>();

        Clear();
    }

    private void Start()
    {
        StartCoroutine(UpdateHighlight());
    }

    public void PlayKeyStroke()
    {
        audioSource.PlayOneShot(KeystrokeSound);
    }

    public void HighlightWords()
    {
        for (int i = activeWordSet.Count() - 1; i >= 0; i--)
        {
            FallingWord fw = activeWordSet._items[i];

            if (inputField.text != "" && fw.Wrapper.Word.Text.StartsWith(inputField.text))
            {
                string part1 = inputField.text;
                string part2 = fw.Wrapper.Word.Text.Substring(part1.Length, fw.Wrapper.Word.Text.Length - part1.Length);

                fw.Text.text = "<color=#" + ColorUtility.ToHtmlStringRGB(highlightColor) + ">" + part1 + "</color>" + part2;
            }
            else
                fw.Text.text = fw.Wrapper.Word.Text;
        }
    }


    public void ConfirmInput()
    {
        if(inputField.text == "") return; 

        ManageActiveWords();

        Clear();
    }

    public void ManageActiveWords()
    {
        PView.RPC("TypedWord", RpcTarget.All, inputField.text);
    }

    private List<FallingWord> getAllActiveWords()
    {
        List<FallingWord> allWords = new List<FallingWord>();

        allWords.AddRange(activeWordSet._items);
        allWords.AddRange(BonusWordSet._items);

        return allWords;
    }

    [PunRPC]
    private void TypedWord(string p_input)
    {
        bool m_foundWord = false;
        foreach(FallingWord word in getAllActiveWords())
        {
            if(word.Check(p_input))
            {
                if (word.tag == "BonusWord")
                {
                    Debug.Log("Sending event code");
                    m_foundWord = true;
                    ScoreHandler.PhotonIncreaseScore(word.GetScore());
                    PhotonNetwork.RaiseEvent((byte)word.Wrapper.Word.EventCode, null, RaiseEventOptions.Default, SendOptions.SendReliable);
                    audioSource.PlayOneShot(PopSound);
                }
                else if(PhotonNetwork.LocalPlayer.ActorNumber == ValidPlayerID)
                {
                    m_foundWord = true;
                    ScoreHandler.PhotonIncreaseScore(word.GetScore());
                    audioSource.PlayOneShot(PopSound);
                }

                word.DestroyWord(false);
            }
        }

        if(PhotonNetwork.LocalPlayer.ActorNumber == ValidPlayerID && !m_foundWord)
        {
            audioSource.PlayOneShot(InvalidSound);
        }
    }

    private void Clear()
    {
        inputField.text = "";
    }

    private IEnumerator UpdateHighlight()
    {
        while (true)
        {
            HighlightWords();
            yield return new WaitForSeconds(0.01f);
        }
    }
}
