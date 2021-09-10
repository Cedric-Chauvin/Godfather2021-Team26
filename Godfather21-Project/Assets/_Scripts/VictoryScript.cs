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
    [SerializeField] Text victoryText;
    [SerializeField] Text BlueVictoryCompt;
    [SerializeField] Text RedVictoryCompt;

    private static int nbVictoryBlue = 0;
    private static int nbVictoryRed = 0;

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
            crown.GetComponent<CrownThrow>().EnemyHasCrown.AddListener(EnemyTookCrown);
            crown.GetComponentInParent<PlayerController>().AllyWithCrownDied.AddListener(AllyWithCrownDied);
            crown.GetComponentInParent<PlayerController>().KingDied.AddListener(KingDied);
        }
    }


    private void RemoveAlly(Pawn ally)
    {
       if(ally.gameObject.CompareTag("Ally"))
       {
            blueAlliesNum--;
            if(blueAlliesNum == 0)
            {
                victoryText.text = "Red defeated Blue's army!";
                Debug.Log("red victory");
                VictoryScreen(1);
            }

       }else if(ally.gameObject.CompareTag("Enemy"))
       {
            redAlliesNum--;
            if(redAlliesNum == 0)
            {
                victoryText.text = "Blue defeated Red's army!";
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

    public void EnemyTookCrown(int i)
    {
        if (i == 0)
            victoryText.text = "Blue took Red's crown";
        else
            victoryText.text = "Red took Blue's crown";
        VictoryScreen(i);
    }
    
    public void AllyWithCrownDied(int i)
    {
        if (i == 0)
            victoryText.text = "Red's crown was lost in battle";
        else
            victoryText.text = "Blue's crown was lost in battle";
        VictoryScreen(i);
    }
    
    public void KingDied(int i)
    {
        if (i == 0)
            victoryText.text = "Red King was killed";
        else
            victoryText.text = "Blue King was killed";
        VictoryScreen(i);
    }

    public void VictoryScreen(int i)
    {
        Time.timeScale = 0;
        victoryScreen.SetActive(true);
        switch (i)
        {
            case 0:  // blue victory
                blueVictoryScreen.SetActive(true);
                nbVictoryBlue += 1;
                break;
            case 1:  // red victory
                redVictoryScreen.SetActive(true);
                nbVictoryRed += 1;
                break;
        }
        BlueVictoryCompt.text = nbVictoryBlue.ToString();
        RedVictoryCompt.text = nbVictoryRed.ToString();
    }

    public void ResetVictoryCount()
    {
        nbVictoryBlue = 0;
        nbVictoryRed = 0;
    }

}

    
