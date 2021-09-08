using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CrownThrow : MonoBehaviour
{

    public UnityEvent<GameObject> allyPickedUpCrown; 
    public Vector2 targetPos;
    public Vector2 kingPos;
    [SerializeField] float speed = 1.5f;
    public bool returnToKing = false;
    public bool stayWithSoldier = false;
    bool hasReachedTarget = false;

    private void Start()
    {
        StartCoroutine(WaitForCrown());
    }

    private void Update()
    {
        if (!stayWithSoldier)
        {
            bool posXReached = transform.localPosition.x <= targetPos.x + .5 && transform.localPosition.x >= targetPos.x - .5;
            bool posYReached = transform.localPosition.y <= targetPos.y + .5 && transform.localPosition.y >= targetPos.y - .5;
            if (posXReached && posYReached)
            {
                hasReachedTarget = true;
            }
            else
            {
                transform.Translate(targetPos.normalized * speed * Time.deltaTime);
            }
        }

        if (returnToKing)
        {
            Vector2 returnPos = kingPos - new Vector2(transform.position.x, transform.position.y /*transform.GetComponentInParent<Transform>().position.x, transform.GetComponentInParent<Transform>().position.y*/);
            transform.Translate(returnPos.normalized* speed * 5 * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.gameObject.CompareTag("Ally") && !returnToKing && hasReachedTarget)
        {
            StopAllCoroutines();
            stayWithSoldier = true;
            allyPickedUpCrown.Invoke(collision.gameObject);
            transform.SetParent(collision.transform); // picked up by soldier
            GetComponent<Collider2D>().enabled = false;
            hasReachedTarget = true;
        }
        else if (collision.gameObject.CompareTag("Player") && hasReachedTarget)
        {
            collision.gameObject.GetComponent<PlayerController>().kingHasCrown = true;
            Destroy(this.gameObject);
        }
    }


    IEnumerator WaitForCrown()
    {
        yield return new WaitForSeconds(3);
        returnToKing = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.gameObject.CompareTag("Ally") && !returnToKing && hasReachedTarget )
        {
            StopAllCoroutines();
            stayWithSoldier = true;
            allyPickedUpCrown.Invoke(collision.gameObject);
            transform.SetParent(collision.transform); // picked up by soldier
            GetComponent<Collider2D>().enabled = false;
            hasReachedTarget = true;
        }
        else if (collision.gameObject.CompareTag("Player") && hasReachedTarget)
        {
            collision.gameObject.GetComponent<PlayerController>().kingHasCrown = true;
            Destroy(this.gameObject);
        }
    }
}
