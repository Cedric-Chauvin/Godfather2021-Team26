using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rewired.Player player;
    [SerializeField] int playerID =0;
    [SerializeField] float maxRange = 7;
    [SerializeField] GameObject target;
    [SerializeField] Collider2D collider;
    [SerializeField] CrownThrow crown;

    Pawn allyWithCrown = null;
    [SerializeField] float soldierSpeed = 2;
    [SerializeField] float rallingRadius = 3;
    [SerializeField] float rallingDuration = 3;
    [SerializeField] float rallingCooldown = 4;
    [SerializeField] Vector2 battledirection;
    [SerializeField] float orderDuration = 3;
    [SerializeField] float orderCooldown = 4;

    private Vector3 throwDirection;
    Vector2 tempDirection;
    public bool kingHasCrown = true;
    bool soldierHasCrown = false;
    private bool fire;
    private bool canRally = true;
    private bool canOrder = true;
    private List<Pawn> teamPawns = new List<Pawn>();
    private Transform mortIcone = null;
    private FeedbackOrder feedbackOrder = null;

    void Awake()
    {
        player = Rewired.ReInput.players.GetPlayer(playerID);
        GetComponent<Animator>().Play("Idle", 0, Random.value);
        mortIcone = transform.GetChild(0);
        feedbackOrder = GetComponentInChildren<FeedbackOrder>();
    }

    private void Start()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag(playerID == 0?"Ally":"Enemy");
        for (int i = 0; i < temp.Length; i++)
        {
            Pawn p = temp[i].GetComponent<Pawn>();
            teamPawns.Add(p);
            p.PawnDeath.AddListener(RemovePawn);
        }
        collider = GetComponent<Collider2D>();
        crown.allyPickedUpCrown.AddListener(AssignSoldier);
    }

    void Update()
    {
        if (kingHasCrown)
        {
            ThrowCrown();
            if (canOrder)
            {
                if (player.GetButtonDown("OrderUp"))
                    Order(VectorUtils.Rotate(battledirection, 90));
                else if (player.GetButtonDown("OrderDown"))  
                    Order(VectorUtils.Rotate(battledirection, -90));
                else if (player.GetButtonDown("OrderRight"))
                    Order(battledirection);
                else if (player.GetButtonDown("OrderLeft"))
                    Order(-battledirection);
            }
        }
        else if (soldierHasCrown)
        {
            if (player.GetButtonDown("Fire"))
            {
                crown.ReturnCrown();
                allyWithCrown.ChangeMoveType(Pawn.MOVEMENT_TYPE.IDLE);
                allyWithCrown.isControlled = false;
                allyWithCrown = null;
                soldierHasCrown = false;
                return;
            }
            if(player.GetButtonDown("Rallying") && canRally)
            {
                Ralling();
            }
            MoveSoldier();
        }
    }

    private void Order(Vector2 direction)
    {
        feedbackOrder.Play(direction, battledirection.x, transform.position.x);
        for (int i = 0; i < teamPawns.Count; i++)
        {
            teamPawns[i].ChangeMoveType(Pawn.MOVEMENT_TYPE.LISTEN, direction, orderDuration);
        }
        StartCoroutine(OrderTimer());
    }

    private void Ralling()
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = false;
        List<Collider2D> result = new List<Collider2D>();
        int nbCollider = Physics2D.OverlapCircle(allyWithCrown.transform.position, rallingRadius, contactFilter, result);
        for (int i = 0; i < nbCollider; i++)
        {
            result[i].GetComponent<Pawn>().ChangeMoveType(Pawn.MOVEMENT_TYPE.REGROUP, allyWithCrown.transform.position, rallingDuration);
        }
        StartCoroutine(RallyTimer());
    }

    public void ThrowCrown()
    {
        throwDirection.x = player.GetAxis("Move Horizontal");
        throwDirection.y = player.GetAxis("Move Vertical");
        fire = player.GetButtonDown("Fire"); 

        if (throwDirection.x != 0.0f || throwDirection.y != 0.0f)
        {
            target.SetActive(true);
            target.transform.position = transform.position + throwDirection.normalized * throwDirection.magnitude * -maxRange;
            tempDirection = throwDirection.normalized * throwDirection.magnitude * -maxRange;
            crown.transform.position = transform.position + throwDirection.normalized;
        }
        else
        {
            crown.transform.position = crown.kingHeadOffset + (Vector2)transform.position;
            target.SetActive(false);
        }

        if (fire)
        {
            kingHasCrown = false;
            crown.targetPos = tempDirection;
            crown.kingPos = transform.position;
            crown.ThrowCrown();

            if (playerID == 0)
            {
                crown.unitTag = "Ally";
            }
            else
            {
                crown.unitTag = "Enemy";
            }
            target.SetActive(false);
        }
    }

    private void RemovePawn(Pawn pawn)
    {
        teamPawns.Remove(pawn);
        if (allyWithCrown == pawn)
            ;//defeat
    }

    public void AssignSoldier(GameObject ally)
    {
        allyWithCrown = ally.GetComponent<Pawn>();
        Debug.Log("got ally :)");
        allyWithCrown.ChangeMoveType(Pawn.MOVEMENT_TYPE.CONTROLED);
        soldierHasCrown = true;
    }

    public void MoveSoldier()
    {
        Vector2 moveDirection = new Vector2(player.GetAxis("Move Horizontal"), player.GetAxis("Move Vertical"));
        allyWithCrown.ControlledMove(moveDirection * soldierSpeed);
    }

    IEnumerator RallyTimer()
    {
        canRally = false;
        yield return new WaitForSeconds(rallingCooldown);
        canRally = true;
    }

    IEnumerator OrderTimer()
    {
        canOrder = false;
        yield return new WaitForSeconds(orderCooldown);
        canOrder = true;
    }
}
