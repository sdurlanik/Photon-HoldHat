using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
     [HideInInspector] public int id;

     [Header("Info")] 
     [SerializeField] private float _moveSpeed;
     [SerializeField] private float _jumpForce;
     [SerializeField] private GameObject hatObject;

     [HideInInspector] public float curHatTime;

     [Header("Components")] 
     [SerializeField] private Rigidbody _rigidbody;
     [SerializeField] private Player _photonPlayer;

     [PunRPC]
     public void Initialize(Player player)
     {
          _photonPlayer = player;
          id = player.ActorNumber;

          GameManager.instance.players[id - 1] = this;

          if (id == 1)
          {
               GameManager.instance.GiveHat(id,true);
          }

          if (!photonView.IsMine)
          {
               _rigidbody.isKinematic = true;
          }
     }
     private void Update()
     {
          Move();

          if (Input.GetKeyDown(KeyCode.Space))
          {
               TryJump();
          }
     }

     void Move()
     {
          float x = Input.GetAxis("Horizontal") * _moveSpeed;
          float z = Input.GetAxis("Vertical") * _moveSpeed;

          _rigidbody.velocity = new Vector3(x, _rigidbody.velocity.y, z);
     }

     void TryJump()
     {
          Ray ray = new Ray(transform.position, Vector3.down);

          if (Physics.Raycast(ray,0.7f))
          {
               _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);     
          }
     }

     public void SetHat(bool hasHat)
     {
          hatObject.SetActive(hasHat);
     }

     void OnCollisionEnter(Collision collision)
     {
          if (!photonView.IsMine)
               return;
          if (collision.gameObject.CompareTag("Player"))
          {
               if (GameManager.instance.GetPlayer(collision.gameObject).id == GameManager.instance.playerWithHat)
               {
                    if (GameManager.instance.CanGetHat())
                    {
                         GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All,id,false);
                    }
               }
          }
     }
}
