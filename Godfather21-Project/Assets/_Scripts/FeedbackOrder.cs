using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackOrder : MonoBehaviour
{
    const int nbFleche = 10;
    const float terainY = 4.5f;
    const float xOffset = 4;
    const float duration = 2;
    const float speed = 1;

    [SerializeField]
    private GameObject prefabUp;

    List<SpriteRenderer> fleches = new List<SpriteRenderer>();
    float timer = 0;
    Vector2 direction = Vector2.zero;

    private void Start()
    {
        for (int i = 0; i < nbFleche; i++)
        {
            SpriteRenderer inst = Instantiate(prefabUp,transform).GetComponent<SpriteRenderer>();
            fleches.Add(inst);
            inst.color = new Color(1,1,1,0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            float dif = duration - timer;
            if (dif < 1) 
            {
                Color color = new Color(1, 1, 1, dif*0.5f);
                foreach (SpriteRenderer item in fleches)
                {
                    item.color = color;
                }
            }
            else if (timer < 1)
            {
                Color color = new Color(1, 1, 1, timer * 0.5f);
                foreach (SpriteRenderer item in fleches)
                {
                    item.color = color;
                }
            }
            
            transform.Translate(direction * speed * Time.deltaTime);
            timer -= Time.deltaTime;
        }
    }

    public void Play(Vector2 orderDirection,float battleDirection,float kingX)
    {
        transform.position = Vector3.zero;
        float rotation = 0;
        if(orderDirection.x>0.1)
            rotation = -90;
        else if (orderDirection.x < -0.1)
            rotation = 90;
        else if (orderDirection.y > 0.1)
            rotation = 0;
        else if (orderDirection.y < -0.1)
            rotation = 180;

        foreach (SpriteRenderer item in fleches)
        {
            item.transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
            Vector2 pos = Vector2.zero;
            pos.x = Random.Range(kingX, kingX + xOffset*battleDirection);
            pos.y = Random.Range(-terainY, terainY);
            item.transform.position = pos;
            item.color = Color.white;
        }
        timer = duration;
        direction = orderDirection;

    }
}
