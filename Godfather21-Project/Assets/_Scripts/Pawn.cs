using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pawn : MonoBehaviour
{
    [SerializeField]
    private Vector2 battleDirection;
    [SerializeField]
    private int maxAngleDirectionIdled = 120;
    [SerializeField]
    private int maxAngleDirectionDirected = 20;
    [SerializeField]
    private float timeBetweenDirectionChange = 2;
    [SerializeField]
    private float battleMaxTime = 10;
    [SerializeField]
    private float walkSpeed = 0.5f;

    private MOVEMENT_TYPE movetype = MOVEMENT_TYPE.IDLE;
    private Vector2 currentDirection;
    private float timer = 0;
    private float battleTimer = 0;
    private Rigidbody2D rgb = null;
    private GameObject Enemy = null;
    private Coroutine coroutine = null;
    [HideInInspector]
    public bool isControlled = false;
    public UnityEvent<Pawn> PawnDeath;

    private void Start()
    {
        rgb = GetComponent<Rigidbody2D>();

    }

    private void OnDestroy()
    {
        PawnDeath.Invoke(this);
    }

    // Update is called once per frame
    private void Update()
    {
        #region DEBUG
        //if (Input.GetMouseButtonDown(0))
        //    ChangeMoveType(MOVEMENT_TYPE.REGROUP, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //if (Input.GetMouseButtonUp(0))
        //    ChangeMoveType(MOVEMENT_TYPE.IDLE);
        //if (Input.GetKeyDown(KeyCode.D))
        //    ChangeMoveType(MOVEMENT_TYPE.LISTEN, Vector2.right);
        //if (Input.GetKeyUp(KeyCode.D))
        //    ChangeMoveType(MOVEMENT_TYPE.IDLE);
        #endregion

        switch (movetype)
        {
            case MOVEMENT_TYPE.IDLE:
                Idle();
                break;
            case MOVEMENT_TYPE.REGROUP:
            case MOVEMENT_TYPE.LISTEN:
                rgb.velocity = currentDirection * walkSpeed;
                break;
            case MOVEMENT_TYPE.BATTLE:
                Battle();
                break;
        }
    }

    private void Idle()
    {
        if (timer > 0)
        {
            rgb.velocity = currentDirection * walkSpeed;
            timer -= Time.deltaTime;
        }
        else
        {
            int angle = Random.Range(-maxAngleDirectionIdled, maxAngleDirectionIdled);
            currentDirection = VectorUtils.Rotate(battleDirection, angle).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDirection, 2,1<<6);
            //Debug.DrawLine(transform.position, (Vector2)transform.position + currentDirection * 2, Color.white, 1);
            if (hit)
            {
                if (Mathf.Abs(angle) > 70)
                    currentDirection = VectorUtils.Rotate(currentDirection, 90 * Mathf.Sign(-angle));
                else
                {
                    Vector2 impactVector = hit.point - (Vector2)hit.transform.position;
                    currentDirection.x = (impactVector.x + hit.normal.x * impactVector.x);
                    currentDirection.y = (impactVector.y + hit.normal.y * impactVector.y);
                    currentDirection = currentDirection.normalized;
                }
            }
            timer = Random.Range(0, timeBetweenDirectionChange);
        }
    }

    private void Battle()
    {
        if(!Enemy)
        {
            movetype = isControlled ? MOVEMENT_TYPE.CONTROLED : MOVEMENT_TYPE.IDLE;
            return;
        }
        rgb.velocity = Vector2.zero;
        if (tag == "Ally") {
            battleTimer -= Time.deltaTime;
            if (battleTimer < 0)
            {
                if (Random.value < 0.5)
                    Destroy(gameObject);
                else
                    Destroy(Enemy);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && tag != "Enemy" || collision.tag == "Ally" && tag != "Ally")
        {
            if (Enemy)
                Destroy(gameObject);
            else
            {
                movetype = MOVEMENT_TYPE.BATTLE;
                Enemy = collision.gameObject;
                battleTimer = battleMaxTime;
                rgb.velocity = Vector2.zero;
            }
            return;
        }
        if(collision.name == "Crown")
        {
            if (!(movetype == MOVEMENT_TYPE.CROWN_DIRECTION))
            {
                currentDirection = ((Vector2)(collision.transform.position - transform.position)).normalized;
                movetype = MOVEMENT_TYPE.CROWN_DIRECTION;
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == Enemy)
        {
            Enemy = null;
            movetype = isControlled ? MOVEMENT_TYPE.CONTROLED : MOVEMENT_TYPE.IDLE;
        }
    }

    public void ChangeMoveType(MOVEMENT_TYPE type , Vector2 vector = default(Vector2),float duration = 0)
    {
        if (movetype == MOVEMENT_TYPE.BATTLE)
            return;
        if (movetype == MOVEMENT_TYPE.CONTROLED && type != MOVEMENT_TYPE.IDLE)
            return;
        movetype = type;
        switch (type)
        {
            case MOVEMENT_TYPE.REGROUP:
                currentDirection = VectorUtils.Rotate((vector - (Vector2)transform.position).normalized,Random.Range(-maxAngleDirectionDirected,maxAngleDirectionDirected));
                if (coroutine != null)
                    StopCoroutine(coroutine);
                coroutine = StartCoroutine(ResetMoveType(duration));
                break;
            case MOVEMENT_TYPE.LISTEN:
                currentDirection = VectorUtils.Rotate(vector.normalized, Random.Range(-maxAngleDirectionDirected, maxAngleDirectionDirected));
                if (coroutine != null)
                    StopCoroutine(coroutine);
                coroutine = StartCoroutine(ResetMoveType(duration));
                break;
            case MOVEMENT_TYPE.CONTROLED:
                if (coroutine != null)
                    StopCoroutine(coroutine);
                isControlled = true;
                break;
        }
    }

    public void ControlledMove(Vector2 direction)
    {
        if(!(movetype == MOVEMENT_TYPE.BATTLE))
            rgb.velocity = direction;
    }

    private IEnumerator ResetMoveType(float time)
    {
        yield return new WaitForSeconds(time);

        movetype = MOVEMENT_TYPE.IDLE;
    }

    public enum MOVEMENT_TYPE
    {
        IDLE,
        REGROUP,
        LISTEN,
        CONTROLED,
        BATTLE,
        CROWN_DIRECTION
    }
}
