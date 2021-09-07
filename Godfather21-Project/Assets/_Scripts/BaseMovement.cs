using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    public Vector2 battleDirection;
    public int maxAngleDirection;
    public float timeBetweenDirectionChange;
    public float walkSpeed;

    private MOVEMENTTYPE movetype = MOVEMENTTYPE.IDLE;
    private Vector2 currentDirection;
    private float timer = 0;
    private Rigidbody2D rgb = null;

    private void Start()
    {
        rgb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    private void Update()
    {

        //#region DEBUG
        //if (Input.GetMouseButtonDown(0))
        //    ChangeMoveType(MOVEMENTTYPE.REGROUP, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //if (Input.GetMouseButtonUp(0))
        //    ChangeMoveType(MOVEMENTTYPE.IDLE);
        //if(Input.GetKeyDown(KeyCode.D))
        //    ChangeMoveType(MOVEMENTTYPE.LISTEN, Vector2.right);
        //if (Input.GetKeyUp(KeyCode.D))
        //    ChangeMoveType(MOVEMENTTYPE.IDLE);
        //#endregion

        switch (movetype)
        {
            case MOVEMENTTYPE.IDLE:
                Idle();
                break;
            case MOVEMENTTYPE.REGROUP:
            case MOVEMENTTYPE.LISTEN:
                rgb.velocity = currentDirection * walkSpeed;
                break;
            case MOVEMENTTYPE.CONTROLED:
                break;
            default:
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
            int r = Random.Range(-maxAngleDirection, maxAngleDirection);
            currentDirection = VectorUtils.rotate(battleDirection, r).normalized;
            timer = Random.Range(0, timeBetweenDirectionChange);
        }
    }

    public void ChangeMoveType(MOVEMENTTYPE type , Vector2 vector = default(Vector2))
    {
        movetype = type;
        switch (type)
        {
            case MOVEMENTTYPE.REGROUP:
                currentDirection = (vector - (Vector2)transform.position).normalized;
                break;
            case MOVEMENTTYPE.LISTEN:
                currentDirection = vector.normalized;
                break;
        }
    }

    public enum MOVEMENTTYPE
    {
        IDLE,
        REGROUP,
        LISTEN,
        CONTROLED
    }
}
