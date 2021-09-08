using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownThrow : MonoBehaviour
{
    public Vector2 targetPos;
    [SerializeField] float speed = 1.5f;

    private void Update()
    {
        Debug.Log(targetPos);
        bool posXReached = transform.position.x <= targetPos.x + .5 && transform.position.x >= targetPos.x - .5;
        bool posYReached = transform.position.y <= targetPos.y + .5 && transform.position.y >= targetPos.y - .5;
        Debug.Log(posXReached +"      "+ posYReached);
        if (posXReached && posYReached)
        {
            //reached target
        }
        else
        {
            transform.Translate(targetPos.normalized * speed * Time.deltaTime);
        }
    }
}
