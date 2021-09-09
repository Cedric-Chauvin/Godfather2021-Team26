using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CrownThrow : MonoBehaviour
{

    public UnityEvent<GameObject> allyPickedUpCrown; 
    public Vector2 targetPos;
    public Vector2 kingPos;
    public Vector2 kingHeadOffset;
    public Vector2 returnPos;
    public float returnToKingTime = 2;
    public bool returnToKing = false;
    public bool stayWithSoldier = false;
    private bool stayWithKing = true;
    bool hasReachedTarget = false;
    List<Pawn> pawnInRange = new List<Pawn>();
    Animator landEffect = null;

    [SerializeField] float aggroRange = 1;
    [SerializeField] float initialSpeed =5;
    [SerializeField] float acc;
    public Vector3 speed;
    public Vector3 pos;
    Animator crownAnim;
    [SerializeField] Collider2D collider;

    public string unitTag;
    private string enemyTag;

    public UnityEvent<int> EnemyHasCrown;

    private void Start()
    {
        crownAnim = GetComponent<Animator>();
        
        enemyTag = unitTag == "Ally"? "Enemy" : "Ally" ;
        crownAnim.SetFloat("PlayerID", unitTag == "Ally" ? 0f : 1f);
        landEffect = transform.GetChild(0).GetComponent<Animator>();
    }

    private void Update()
    {
        if (returnToKing)
        {
            transform.Translate(returnPos.normalized * 7.5f * Time.deltaTime);
        }

    }
    private void FixedUpdate()
    {
        if (!stayWithSoldier && !hasReachedTarget && !stayWithKing)
        {
            bool posXReached = transform.localPosition.x <= targetPos.x + .5 && transform.localPosition.x >= targetPos.x - .5;
            bool posYReached = transform.localPosition.y <= targetPos.y + .5 && transform.localPosition.y >= targetPos.y - .5;
            if ((posXReached && posYReached) || (speed.magnitude < .1f && speed.magnitude > -.1f))
            {
                hasReachedTarget = true;
                acc = 0;
                crownAnim.SetTrigger("OnGround");
                landEffect.Play("Land",0,0);
                ContactFilter2D contactFilter = new ContactFilter2D();
                contactFilter.useTriggers = false;
                List<Collider2D> result = new List<Collider2D>();
                int nbCollider = Physics2D.OverlapCircle(transform.position, aggroRange, contactFilter, result);
                for (int i = 0; i < nbCollider; i++)
                {
                    Pawn p = result[i].GetComponent<Pawn>();
                    if (p)
                    {
                        pawnInRange.Add(p);
                        p.ChangeMoveType(Pawn.MOVEMENT_TYPE.CROWN_DIRECTION, transform.position);
                    }
                }
            }
            else
            {
                pos += new Vector3(speed.x * Time.fixedDeltaTime + targetPos.x * acc * Mathf.Pow(Time.fixedDeltaTime, 2) / 2, speed.y * Time.fixedDeltaTime + targetPos.y * acc * Mathf.Pow(Time.fixedDeltaTime, 2) / 2, 0);
                speed += new Vector3(targetPos.x * acc * Time.fixedDeltaTime, targetPos.y * acc * Time.fixedDeltaTime, 0);
                this.transform.position = pos;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckCollisions(collision);
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        CheckCollisions(collision);
    }


    private void CheckCollisions(Collider2D collision)
    {
        if (!collision.isTrigger && collision.gameObject.CompareTag(unitTag) && !returnToKing && hasReachedTarget && !stayWithSoldier)
        {
            StopAllCoroutines();
            crownAnim.SetTrigger("PickedUp");
            stayWithSoldier = true;
            allyPickedUpCrown.Invoke(collision.gameObject);
            transform.SetParent(collision.transform); // picked up by soldier
            transform.localPosition = new Vector2(0, 0.7f);
            collider.enabled = false;
            pawnInRange.Remove(collision.GetComponent<Pawn>());
            pawnInRange.ForEach(p => p.ChangeMoveType(Pawn.MOVEMENT_TYPE.IDLE));
            pawnInRange.Clear();
            landEffect.Play("Land", 0, 1); ;
        }
        else if (collision.gameObject.CompareTag("Player") && hasReachedTarget)
        {
            returnToKing = false;
            collider.enabled = false;
            collision.gameObject.GetComponent<PlayerController>().CrownToggle(true);
            stayWithKing = true;
            crownAnim.SetTrigger("PickedUp");
            speed = new Vector2(0,0);
            acc = 0;
            transform.SetParent(collision.transform);
            transform.localPosition = new Vector2(0, 0) + kingHeadOffset;
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            hasReachedTarget = true;
            crownAnim.SetTrigger("OnGround");
        }
        else if(!collision.isTrigger && collision.gameObject.CompareTag(enemyTag) && !returnToKing && hasReachedTarget)
        {
            int i = enemyTag == "Ally" ? 0 : 1;
            EnemyHasCrown.Invoke(i);
        }
    }

    IEnumerator WaitForCrown()
    {
        yield return new WaitForSeconds(3);
        crownAnim.SetTrigger("ThrowCrown");
        returnPos = (kingPos - new Vector2(transform.position.x, transform.position.y)) /returnToKingTime;
        returnToKing = true; 
    }

    public void ReturnCrown()
    {
        returnPos = (kingPos - new Vector2(transform.position.x, transform.position.y)) / returnToKingTime *2;
        returnToKing = true;
        crownAnim.SetTrigger("ThrowCrown");
        collider.enabled = true;
        stayWithSoldier = false;
        transform.SetParent(null);
    }

    public void ThrowCrown()
    {
        StartCoroutine(WaitForCrown());
        hasReachedTarget = false;
        collider.enabled = true;
        stayWithKing = false;
        crownAnim.SetTrigger("ThrowCrown");
        speed = targetPos.normalized * (new Vector2(targetPos.x - transform.position.x, targetPos.y - transform.position.y).magnitude / 20) * initialSpeed;
        pos = transform.position;
        acc = -.8f;
        /*acc = - new Vector2(targetPos.x - transform.position.x, targetPos.y - transform.position.y).magnitude /25;
        acc = Mathf.Clamp(acc, -.7f, -.1f);
        Debug.Log(new Vector2(targetPos.x - transform.position.x, targetPos.y - transform.position.y).magnitude  + "   ---   " + acc);*/
    }
}
