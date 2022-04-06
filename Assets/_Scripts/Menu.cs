using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class Menu : MonoBehaviourPunCallbacks
{
   [Header("Screens")]
   [SerializeField]private GameObject _mainScreen;
   [SerializeField]private GameObject _lobbyScreen;

   [Header("Main Screen")] 
   [SerializeField]private Button _createRoomButton;
   [SerializeField]private Button _joinRoomButton;

   [Header("Lobby Screen")] 
   [SerializeField]private TextMeshProUGUI _playerListText;
   [SerializeField]private Button _startGameButton;

   private void Start()
   {
      _createRoomButton.interactable = false;
      _joinRoomButton.interactable = false;
   }

   public override void OnConnectedToMaster()
   {
      _createRoomButton.interactable = true;
      _joinRoomButton.interactable = true;
   }

   void SetScreen(GameObject screen)
   {
      _mainScreen.SetActive(false);
      _lobbyScreen.SetActive(false);
      
      screen.SetActive(true);
   }

   public void onCreateRoomButton(TMP_InputField roomNameInput)
   {
      NetworkManager.instance.CreateRoom(roomNameInput.text);
   }

   public void onJoinRoomButton(TMP_InputField roomNameInput)
   {
      NetworkManager.instance.JoinRoom(roomNameInput.text);
   }

   public void onPlayerNameUpdate(TMP_InputField playerNameInput)
   {
      PhotonNetwork.NickName = playerNameInput.text;
   }

   public override void OnJoinedRoom()
   {
      SetScreen(_lobbyScreen);
      
      photonView.RPC("UpdateLobbyUI", RpcTarget.All);
   }

   public override void OnPlayerLeftRoom(Player otherPlayer)
   {
      UpdateLobbyUI();
   }

   [PunRPC]
   public void UpdateLobbyUI()
   {
      _playerListText.text = "";

      foreach (Player player in PhotonNetwork.PlayerList )
      {
         _playerListText.text += player.NickName + "\n";
      }

      if (PhotonNetwork.IsMasterClient)
         _startGameButton.interactable = true;
      else
         _startGameButton.interactable = false;
   }

   public void OnLeaveLobbyButton()
   {
      PhotonNetwork.LeaveRoom();
      SetScreen(_mainScreen);
   }

   public void OnStartGameButton()
   {
      NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
   }
}
