using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rewired.Player player;
    [SerializeField] GameObject target;
    [SerializeField] GameObject crown;
    [SerializeField] private Vector3 throwDirection;
    [SerializeField] List<Vector2> tempDirection;
    bool kingHasCrown = true;
    bool soldierHasCrown = false;
    private bool fire;

    void Awake()
    {
        player = Rewired.ReInput.players.GetPlayer(0);
        tempDirection.Add(new Vector2());
        tempDirection.Add(new Vector2());
        tempDirection.Add(new Vector2());
        tempDirection.Add(new Vector2());
        tempDirection.Add(new Vector2());
    }

    void Update()
    {
        if (kingHasCrown)
        {
            ThrowCrown();
        }
    }

    public void ThrowCrown()
    {
        //kingHasCrown = false;
        throwDirection.x = player.GetAxis("Move Horizontal");
        throwDirection.y = player.GetAxis("Move Vertical");
        fire = player.GetButtonDown("Fire"); 

        if (throwDirection.x != 0.0f || throwDirection.y != 0.0f)
        {
            target.SetActive(true);
            target.transform.position = transform.position + throwDirection.normalized * throwDirection.magnitude * -5;
            for (int i = 0; i < 4; i++)
            {
                tempDirection[i] = tempDirection[i + 1];
            }
            tempDirection[4] = throwDirection.normalized * throwDirection.magnitude * -5;
        }
        else if (target.activeSelf)
        {
            GameObject obj = Instantiate(crown, transform);
            obj.GetComponent<CrownThrow>().targetPos = tempDirection[0];
            Debug.Log(tempDirection[0]);
            target.SetActive(false);
        }
        //StartCoroutine(WaitForCrown());
    }

    IEnumerator WaitForCrown()
    {
        yield return new WaitForSeconds(3);
        if (!soldierHasCrown)
        {
            kingHasCrown = true;
        }
    }
}
