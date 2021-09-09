using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    private bool kingHasCrown = true;
    bool soldierHasCrown = false;
    private bool fire;
    private bool canRally = true;
    private bool canOrder = true;
    private List<Pawn> teamPawns = new List<Pawn>();
    private Transform mortIcone = null;
    private FeedbackOrder feedbackOrder = null;
    private Animator animator = null;
    private Animator powerUp = null;

    public UnityEvent<int> KingDied;
    public UnityEvent<int> AllyWithCrownDied;

    
    //SFX
    AudioSource audio;
    bool soundIsPlaying;
    [SerializeField] List<AudioClip> audioClips;

    void Awake()
    {
        player = Rewired.ReInput.players.GetPlayer(playerID);
        mortIcone = transform.GetChild(0);
        crown.unitTag = playerID == 0 ? "Ally" : "Enemy";
        feedbackOrder = GetComponentInChildren<FeedbackOrder>();
        animator = GetComponent<Animator>();
        powerUp = transform.GetChild(3).GetComponent<Animator>();
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
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!GameManager.Instance.gamePaused)
        {
            if (kingHasCrown)
            {
                ThrowCrown();
                if (canOrder)
                {
                    if (player.GetButtonDown("OrderUp"))
                        Order(Vector2.up);
                    else if (player.GetButtonDown("OrderDown"))
                        Order(Vector2.down);
                    else if (player.GetButtonDown("OrderRight"))
                        Order(Vector2.right);
                    else if (player.GetButtonDown("OrderLeft"))
                        Order(Vector2.left);
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
                if (player.GetButtonDown("Rallying") && canRally)
                {
                    Ralling();
                }
                MoveSoldier();
            }
        }
    }

    private void Order(Vector2 direction)
    {
        feedbackOrder.Play(direction, battledirection.x, transform.position.x);
        for (int i = 0; i < teamPawns.Count; i++)
        {
            teamPawns[i].ChangeMoveType(Pawn.MOVEMENT_TYPE.LISTEN, direction, orderDuration);
            audio.clip = audioClips[3];
            audio.Play();
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
            Pawn p = result[i].GetComponent<Pawn>();
            if(p)
                p.Rallying(allyWithCrown.transform, rallingDuration);
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
            crown.transform.position = transform.position + throwDirection.normalized * throwDirection.magnitude/2;
            if (!soundIsPlaying)
            {
                soundIsPlaying = true;
                audio.clip = audioClips[0];
                audio.Play();
            }
            player.SetVibration(0, tempDirection.magnitude / maxRange);
        }
        else
        {
            player.SetVibration(0, 0);
            tempDirection = Vector2.zero;
            crown.transform.position = crown.kingHeadOffset + (Vector2)transform.position;
            target.SetActive(false);
            soundIsPlaying = false;
        }

        if (fire && tempDirection.magnitude>1.5f)
        {
            player.SetVibration(0, 0);
            kingHasCrown = false;
            if(CapacityUI.UI)
                CapacityUI.UI.CrownToggle(playerID, false);
            animator.SetBool("Crowned", false);
            crown.targetPos = tempDirection;
            crown.kingPos = transform.position;
            crown.ThrowCrown();
            target.SetActive(false);
            audio.clip = audioClips[1];
            audio.Play();
        }
    }

    private void RemovePawn(Pawn pawn)
    {
        teamPawns.Remove(pawn);
        if (allyWithCrown == pawn) {
            int i = playerID == 0 ? 1 : 0;
            AllyWithCrownDied.Invoke(i) ;//defeat
        }
    }

    public void AssignSoldier(GameObject ally)
    {
        allyWithCrown = ally.GetComponent<Pawn>();
        allyWithCrown.ChangeMoveType(Pawn.MOVEMENT_TYPE.CONTROLED);
        soldierHasCrown = true;
    }

    public void MoveSoldier()
    {
        Vector2 moveDirection = new Vector2(player.GetAxis("MovePawnHorizontal"), player.GetAxis("MovePawnVertical"));
        allyWithCrown.ControlledMove(moveDirection * soldierSpeed);
    }

    public void CrownToggle(bool b)
    {
        animator.SetBool("Crowned", b);
        kingHasCrown = b;
        if(CapacityUI.UI)
            CapacityUI.UI.CrownToggle(playerID, b);
        if (b)
            powerUp.Play("Play", 0, 0);
    }

    IEnumerator RallyTimer()
    {
        CapacityUI.UI.RallyUsable(playerID, false);
        canRally = false;
        yield return new WaitForSeconds(rallingCooldown);
        canRally = true;
        CapacityUI.UI.RallyUsable(playerID, true);
    }

    IEnumerator OrderTimer()
    {
        canOrder = false;
        CapacityUI.UI.OrderUsable(playerID, false);
        yield return new WaitForSeconds(orderCooldown);
        canOrder = true;
        CapacityUI.UI.OrderUsable(playerID, true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string enemyTag = playerID == 0 ? "Enemy" : "Ally";
        if (collision.gameObject.CompareTag(enemyTag)){
            int i = playerID == 0 ? 1 : 0;
            audio.clip = audioClips[2];
            audio.Play();
            KingDied.Invoke(i);
        }
    }
}
