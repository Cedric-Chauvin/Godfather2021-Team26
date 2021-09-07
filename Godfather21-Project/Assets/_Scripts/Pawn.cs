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

    private MOVEMENTTYPE movetype = MOVEMENTTYPE.IDLE;
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
        //if (Input.GetMouseButtonDown(0))
        //    ChangeMoveType(MOVEMENTTYPE.REGROUP, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //if (Input.GetMouseButtonUp(0))
        //    ChangeMoveType(MOVEMENTTYPE.IDLE);
        //if (Input.GetKeyDown(KeyCode.D))
        //    ChangeMoveType(MOVEMENTTYPE.LISTEN, Vector2.right);
        //if (Input.GetKeyUp(KeyCode.D))
        //    ChangeMoveType(MOVEMENTTYPE.IDLE);
        #endregion

        switch (movetype)
        {
            case MOVEMENTTYPE.IDLE:
                Idle();
                break;
            case MOVEMENTTYPE.REGROUP:
            case MOVEMENTTYPE.LISTEN:
                rgb.velocity = currentDirection * walkSpeed;
                break;
            case MOVEMENTTYPE.BATTLE:
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
            int r = Random.Range(-maxAngleDirectionIdled, maxAngleDirectionIdled);
            currentDirection = VectorUtils.Rotate(battleDirection, r).normalized;
            timer = Random.Range(0, timeBetweenDirectionChange);
        }
    }

    private void Battle()
    {
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
                ChangeMoveType(MOVEMENTTYPE.BATTLE);
                Enemy = collision.gameObject;
                battleTimer = battleMaxTime;
                rgb.velocity = Vector2.zero;
            }
            return;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == Enemy)
        {
            Enemy = null;
            ChangeMoveType(MOVEMENTTYPE.IDLE);
        }
    }

    public void ChangeMoveType(MOVEMENTTYPE type , Vector2 vector = default(Vector2))
    {
        movetype = type;
        switch (type)
        {
            case MOVEMENTTYPE.REGROUP:
                currentDirection = VectorUtils.Rotate((vector - (Vector2)transform.position).normalized,Random.Range(-maxAngleDirectionDirected,maxAngleDirectionDirected));
                break;
            case MOVEMENTTYPE.LISTEN:
                currentDirection = VectorUtils.Rotate(vector.normalized, Random.Range(-maxAngleDirectionDirected, maxAngleDirectionDirected));
                break;
        }
    }

    public enum MOVEMENTTYPE
    {
        IDLE,
        REGROUP,
        LISTEN,
        CONTROLED,
        BATTLE
    }
}
