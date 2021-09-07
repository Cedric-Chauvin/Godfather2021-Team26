using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rewired.Player player;
    private CharacterController cc;
    private Vector3 moveVector;
    private bool fire;

    void Awake()
    {
        player = Rewired.ReInput.players.GetPlayer(0);
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        moveVector.x = player.GetAxis("Move Horizontal"); 
        moveVector.y = player.GetAxis("Move Vertical");
        fire = player.GetButtonDown("Fire"); 
        
        if (moveVector.x != 0.0f || moveVector.y != 0.0f)
        {
            cc.Move(moveVector * 2 * Time.deltaTime);
        }
        if (fire)
        {
            Debug.Log("pressed A ");
        }
    }

}
