using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScript : MonoBehaviour
{
    int blueAlliesNum;
    int redAlliesNum;
    public Slider numberBlueRed;
    [SerializeField] GameObject blueVictoryScreen;
    [SerializeField] GameObject redVictoryScreen;
    [SerializeField] GameObject victoryScreen;

    void Start()
    {
        GameObject[] blueAllies = GameObject.FindGameObjectsWithTag("Ally");
        blueAlliesNum = blueAllies.Length;

        GameObject[] redAllies = GameObject.FindGameObjectsWithTag("Enemy");
        redAlliesNum = redAllies.Length;

        foreach (GameObject blue in blueAllies)
        {
            blue.GetComponent<Pawn>().PawnDeath.AddListener(RemoveAlly);
        }
        
        foreach (GameObject red in redAllies)
        {
            red.GetComponent<Pawn>().PawnDeath.AddListener(RemoveAlly);
        }
        UpdateSlider();

        GameObject[] crowns = GameObject.FindGameObjectsWithTag("Crown");
        foreach (GameObject crown in crowns)
        {
            crown.GetComponent<CrownThrow>().EnemyHasCrown.AddListener(VictoryScreen);
            crown.GetComponentInParent<PlayerController>().Defeat.AddListener(VictoryScreen);
        }
    }


    private void RemoveAlly(Pawn ally)
    {
       if(ally.gameObject.CompareTag("Ally"))
       {
            blueAlliesNum--;
            if(blueAlliesNum == 0)
            {
                Debug.Log("red victory");
                VictoryScreen(1);
            }

       }else if(ally.gameObject.CompareTag("Enemy"))
       {
            redAlliesNum--;
            if(redAlliesNum == 0)
            {
                Debug.Log("blue victory");
                VictoryScreen(0);
            }
       }
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        numberBlueRed.maxValue = redAlliesNum + blueAlliesNum;
        numberBlueRed.value = blueAlliesNum;
    }


    public void VictoryScreen(int i)
    {
        Time.timeScale = 0;
        victoryScreen.SetActive(true);
        switch (i)
        {
            case 0:  // blue victory
                blueVictoryScreen.SetActive(true);
                break;
            case 1:  // red victory
                redVictoryScreen.SetActive(true);
                break;
        }

    }

}

    
