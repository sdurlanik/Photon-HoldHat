using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")] 
    [SerializeField] private float _invincibleDuration;
    private float _hatPickupTime;
    public float timeToWin;
    public bool gameEnded = false;
 


    [Header("Players")] 
    [SerializeField] private string _playerPrefabLocation;
    [SerializeField] private Transform[] _spawnPoints;
    private int _playersInGame;
    public PlayerController[] players;
    public int playerWithHat;


    public PhotonView photonView;

    public static GameManager instance;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ImInGame()
    {
        _playersInGame++;
        if (_playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();    
        }
    }

    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(_playerPrefabLocation,
            _spawnPoints[Random.Range(0, _spawnPoints.Length)].position, Quaternion.identity);
        
        PlayerController playerScript = playerObj.GetComponent<PlayerController>();
        
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId);
    }

    public PlayerController GetPlayer(GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }

    [PunRPC]
    public void GiveHat(int playerId, bool initialGive)
    {
        if (!initialGive)
            GetPlayer(playerWithHat).SetHat(false);
        
        playerWithHat = playerId;
        GetPlayer(playerId).SetHat(true);
        _hatPickupTime = Time.time;
    }

    public bool CanGetHat()
    {
        if (Time.time > _hatPickupTime + _invincibleDuration)
            return true;
        else
            return false;
    }

    [PunRPC]
    void WinGame(int playerId)
    {
        gameEnded = true;
        PlayerController player = GetPlayer(playerId);
        
        GameUI.instance.SetWinText(player.photonPlayer.NickName);
        
        Invoke("GoBackToMenu",3);
    }

    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }
}
