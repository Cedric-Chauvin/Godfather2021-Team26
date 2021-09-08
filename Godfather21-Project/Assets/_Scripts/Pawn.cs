using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    public Vector2 battleDirection;
    public int maxAngleDirectionIdled = 120;
    public int maxAngleDirectionDirected = 20;
    public float timeBetweenDirectionChange = 2;
    public float battleMaxTime = 10;
    public float walkSpeed = 0.5f;

    private MOVEMENT_TYPE movetype = MOVEMENT_TYPE.IDLE;
    private Vector2 currentDirection;
    private float timer = 0;
    private float battleTimer = 0;
    private Rigidbody2D rgb = null;
    private GameObject Enemy = null;

    private void Start()
    {
        rgb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    private void Update()
    {
        #region DEBUG
        if (Input.GetMouseButtonDown(0))
            ChangeMoveType(MOVEMENT_TYPE.REGROUP, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (Input.GetMouseButtonUp(0))
            ChangeMoveType(MOVEMENT_TYPE.IDLE);
        if (Input.GetKeyDown(KeyCode.D))
            ChangeMoveType(MOVEMENT_TYPE.LISTEN, Vector2.right);
        if (Input.GetKeyUp(KeyCode.D))
            ChangeMoveType(MOVEMENT_TYPE.IDLE);
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
            case MOVEMENT_TYPE.CONTROLED:
                rgb.velocity = Vector2.zero;
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
            int r = Random.Range(-maxAngleDirectionIdled, maxAngleDirectionIdled);
            currentDirection = VectorUtils.Rotate(battleDirection, r).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDirection, 2,1<<6);
            Debug.DrawLine(transform.position, (Vector2)transform.position + currentDirection * 2,Color.white,1);
            if (hit)
            {
                Vector2 impactVector = hit.point - (Vector2)hit.transform.position;
                currentDirection.x = (impactVector.x + hit.normal.x * impactVector.x);
                currentDirection.y = (impactVector.y + hit.normal.y * impactVector.y);
                currentDirection = currentDirection.normalized;
            }
            timer = Random.Range(0, timeBetweenDirectionChange);
        }
    }

    private void Battle()
    {
        if(!Enemy)
        {
            movetype = MOVEMENT_TYPE.IDLE;
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
                ChangeMoveType(MOVEMENT_TYPE.BATTLE);
                Enemy = collision.gameObject;
                battleTimer = battleMaxTime;
                rgb.velocity = Vector2.zero;
            }
            return;
        }
        if(tag == "Ally" && collision.name == "Crown")
        {
            if (movetype == MOVEMENT_TYPE.CROWN_DIRECTION)
            {
                movetype = MOVEMENT_TYPE.CONTROLED;
                collision.transform.parent = transform;
            }
            else
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
            ChangeMoveType(MOVEMENT_TYPE.IDLE);
        }
    }

    public void ChangeMoveType(MOVEMENT_TYPE type , Vector2 vector = default(Vector2))
    {
        movetype = type;
        switch (type)
        {
            case MOVEMENT_TYPE.REGROUP:
                currentDirection = VectorUtils.Rotate((vector - (Vector2)transform.position).normalized,Random.Range(-maxAngleDirectionDirected,maxAngleDirectionDirected));
                break;
            case MOVEMENT_TYPE.LISTEN:
                currentDirection = VectorUtils.Rotate(vector.normalized, Random.Range(-maxAngleDirectionDirected, maxAngleDirectionDirected));
                break;
        }
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
