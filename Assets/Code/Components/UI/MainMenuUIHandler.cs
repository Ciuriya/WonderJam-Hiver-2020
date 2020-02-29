﻿using UnityEngine;
using System.Collections;
using TMPro;
using Photon.Pun;

/*
 * Handles all UI code calls in the main menu.
 */
public class MainMenuUIHandler : MonoBehaviour
{
    [Tooltip("The bridge between the multiplayer session and the game")]
    public MultiplayerConnector Connector;

    [Tooltip("The input field for the room name")]
    public TMP_InputField RoomInputField;

    [Tooltip("The main menu's main UI")]
    public GameObject MainUI;

    [Tooltip("The main menu's room selection UI")]
    public GameObject SelectRoomUI;

    [Tooltip("The main menu's waiting for player UI")]
    public GameObject WaitingForPlayerUI;

    [Tooltip("The text displaying the current room's name")]
    public TMP_Text CurrentRoomNameText;

    void Awake()
    {
        MainUI.SetActive(true);
        SelectRoomUI.SetActive(false);
        WaitingForPlayerUI.SetActive(false);

        RoomInputField.text = "";
    }

    public void EnterRoom()
    {
        Connector.JoinRoom(RoomInputField.text);
    }

    public void RoomJoined()
    {
        SelectRoomUI.SetActive(false);
        WaitingForPlayerUI.SetActive(true);

        CurrentRoomNameText.text = "Room: " + PhotonNetwork.CurrentRoom.Name;

        if(PhotonNetwork.CurrentRoom.PlayerCount < 2)
            StartCoroutine(WaitForPlayer());
        else PhotonNetwork.LoadLevel(1);
    }

    private IEnumerator WaitForPlayer()
    {
        while(PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            yield return new WaitForSecondsRealtime(0.5f);
        }

        if(PhotonNetwork.InRoom) PhotonNetwork.LoadLevel(1);
    }

    public void RoomFail()
    {
        StartCoroutine(InputFieldError());
    }

    private IEnumerator InputFieldError()
    {
        RoomInputField.interactable = false;
        RoomInputField.text = "Joining room failed!";

        float percentage = 1f;

        while(percentage > 0f)
        {
            RoomInputField.textComponent.color = new Color(1, 0, 0, percentage);
            percentage -= 0.05f;

            yield return new WaitForSeconds(0.1f);
        }

        RoomInputField.text = "";
        RoomInputField.textComponent.color = new Color(50f / 255f, 50f / 255f, 50f / 255f);
        RoomInputField.interactable = true;
    }

    public void RoomInputFieldChanged(string p_input)
    {
        RoomInputField.text = p_input;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
