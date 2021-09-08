using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rewired.Player player;
    [SerializeField] GameObject target;
    [SerializeField] Collider2D collider;
    [SerializeField] GameObject crownPrefab;
    CrownThrow crown;

    GameObject allyWithCrown = null;
    [SerializeField] float soldierSpeed = 2;

    private Vector3 throwDirection;
    Vector2 tempDirection;
    public bool kingHasCrown = true;
    bool soldierHasCrown = false;
    private bool fire;

    void Awake()
    {
        player = Rewired.ReInput.players.GetPlayer(0);
    }

    private void Start()
    {

        collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (kingHasCrown)
        {
            ThrowCrown();
        }
        else if (soldierHasCrown)
        {
            if (player.GetButtonDown("Fire"))
            {
                crown.GetComponent<CrownThrow>().returnToKing = true;
                crown.GetComponent<Collider2D>().enabled = true;
                allyWithCrown.GetComponent<Pawn>().ChangeMoveType(Pawn.MOVEMENT_TYPE.IDLE);
                allyWithCrown = null;
                soldierHasCrown = false;
                return;
            }
            MoveSoldier();
        }
    }

    public void ThrowCrown()
    {
        throwDirection.x = player.GetAxis("Move Horizontal");
        throwDirection.y = player.GetAxis("Move Vertical");
        fire = player.GetButtonDown("Fire"); 

        if (throwDirection.x != 0.0f || throwDirection.y != 0.0f)
        {
            target.SetActive(true);
            target.transform.position = transform.position + throwDirection.normalized * throwDirection.magnitude * -5;
            /*
            for (int i = 0; i < 4; i++)
            {
                tempDirection[i] = tempDirection[i + 1];
            }
            */
            tempDirection = throwDirection.normalized * throwDirection.magnitude * -5;
        }
         if (fire)
        {
            kingHasCrown = false;
            crown = Instantiate(crownPrefab, transform).GetComponent<CrownThrow>();
            crown.targetPos = tempDirection;
            crown.allyPickedUpCrown.AddListener(AssignSoldier);
            crown.kingPos = transform.position;
            target.SetActive(false);
        }
    }


    public void AssignSoldier(GameObject ally)
    {
        allyWithCrown = ally;
        Debug.Log("got ally :)");
        allyWithCrown.GetComponent<Pawn>().ChangeMoveType(Pawn.MOVEMENT_TYPE.CONTROLED);
        soldierHasCrown = true;
    }

    public void MoveSoldier()
    {
        Vector2 moveDirection = new Vector2(player.GetAxis("Move Horizontal"), player.GetAxis("Move Vertical"));
        allyWithCrown.transform.Translate(moveDirection * soldierSpeed * Time.deltaTime);
    }
}
