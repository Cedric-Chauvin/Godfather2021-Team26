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

    [SerializeField] float initialSpeed =5;
    [SerializeField] float acc;
    public Vector3 speed;
    public Vector3 pos;
    Animator crownAnim;
    [SerializeField] Collider2D collider;

    public string unitTag;

    private void Start()
    {
        crownAnim = GetComponent<Animator>();
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
        if (!collision.isTrigger && collision.gameObject.CompareTag(unitTag) && !returnToKing && hasReachedTarget)
        {
            StopAllCoroutines();
            crownAnim.SetTrigger("PickedUp");
            stayWithSoldier = true;
            allyPickedUpCrown.Invoke(collision.gameObject);
            transform.SetParent(collision.transform); // picked up by soldier
            transform.localPosition = new Vector2(0, 0);
            collider.enabled = false;
        }
        else if (collision.gameObject.CompareTag("Player") && hasReachedTarget)
        {
            returnToKing = false;
            collider.enabled = false;
            collision.gameObject.GetComponent<PlayerController>().kingHasCrown = true;
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
