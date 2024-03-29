﻿using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MultiplayerEventSystem : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public BonusHandler[] Players = new BonusHandler[2];

    public GameObject content;

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.CustomData == null || !(photonEvent.CustomData is int))
            return;

        int eventCode = photonEvent.Code;
        int attackingPlayer = (int) photonEvent.CustomData;
        int self = attackingPlayer == 1 ? 0 : 1;
        int other = attackingPlayer == 1 ? 1 : 0;

        switch(eventCode)
        {
            case 0: 
                break;
            case 1:
                string bugText = "";

                for(int i = 0; i < 5; i++)
                    bugText += (char) Random.Range(33, 127);

                Players[other].SpawnWords(0, bugText);
                break;
            case 2: 
                Players[other].AddReverseWords(3);
                break;
            case 3: 
                Players[self].FreezeActiveWords();
                break;
            case 4:
                string[] loopedWord = new string[5];

                for(int i = 0; i < 5; i++)
                    loopedWord[i] = "loop";

                Players[self].SpawnWords(2, loopedWord);
                break;
            case 5:
                Players[self].AddScore(25);
                break;
            case 6:
                Players[other].AddFunctionWords(3);
                break;
            case 7:
                Players[other].AddCommentWords(3);
                break;
            case 8:
                Players[self].ApplyMultSpeedToActiveWords(0.35f);
                Players[self].AddSlowdownWords(3);
                break;
            case 9:
                Players[other].ApplyMultSpeedToActiveWords(1.5f);
                Players[other].AddSpeedUpWords(3);
                break;
            case 10:
                Players[other].TransformAllWordsIntoWord("revert", 2);
                break;
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        StartCoroutine(FailedRoomListJoin(gameObject.GetComponent<MultiplayerConnector>().clickedRoomListEntry, message));
    }

    private IEnumerator FailedRoomListJoin(GameObject p_button, string message)
    {
        string textToPutBack = p_button.GetComponentInChildren<TMP_Text>().text;
        p_button.GetComponentInChildren<TMP_Text>().text = message;
        Color colorToPutBack = p_button.GetComponentInChildren<TMP_Text>().color;
        float percentage = 1f;

        while (percentage > 0f)
        {
            p_button.GetComponentInChildren<TMP_Text>().color = new Color(1, 0, 0, percentage);
            percentage -= 0.05f;

            yield return new WaitForSeconds(0.1f);
        }

        p_button.GetComponentInChildren<TMP_Text>().text = textToPutBack;
        p_button.GetComponentInChildren<TMP_Text>().color = colorToPutBack;
    }
}