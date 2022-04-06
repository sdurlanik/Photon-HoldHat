using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
     [HideInInspector] public int id;

     [Header("Info")] 
     [SerializeField] private float _moveSpeed;
     [SerializeField] private float _jumpSpeed;
     [SerializeField] private GameObject hatObject;

     [HideInInspector] public float curHatTime;

     [Header("Components")] 
     [SerializeField] private Rigidbody _rigidbody;
     [SerializeField] private Player _photonPlayer;

}
