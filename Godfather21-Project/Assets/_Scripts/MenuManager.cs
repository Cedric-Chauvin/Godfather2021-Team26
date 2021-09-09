using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    int currentPosition = 0;
    [SerializeField] List<Button> buttons;
    Player player0;
    Player player1;
    [SerializeField] RectTransform cursor;
    bool cursorMoving = false;

    private void Start()
    {
        player0 = Rewired.ReInput.players.GetPlayer(0);
        player1 = Rewired.ReInput.players.GetPlayer(1);
        
    }
    private void Update()
    {
        if (!cursorMoving && player0.GetAxis("Move Vertical") <= -0.2 || player1.GetAxis("Move Vertical") <= -0.2)
        {
            cursorMoving = true;
            currentPosition++;
            if (currentPosition == buttons.Count)
            {
                currentPosition = 0;
            }else if(currentPosition < 0)
            {
                currentPosition = buttons.Count - 1;
            }
            UpdateButtons();
        }else if(!cursorMoving && player0.GetAxis("Move Vertical") >= 0.2 || player1.GetAxis("Move Vertical") >= 0.2)
        {
            cursorMoving = true;
            currentPosition--;
            if (currentPosition == buttons.Count)
            {
                currentPosition = 0;
            }
            else if (currentPosition < 0)
            {
                currentPosition = buttons.Count - 1;
            }
            UpdateButtons();
        }

        if (player0.GetButtonDown("Fire"))
        {
            SelectedButtonPressed();
        }
    }

    public void UpdateButtons()
    {
        StartCoroutine(WaitForCursor());
        cursor.position = buttons[currentPosition].GetComponent<RectTransform>().position;
    }

    public void SelectedButtonPressed()
    {
        buttons[currentPosition].onClick.Invoke();
    }


    IEnumerator WaitForCursor()
    {
        yield return new WaitForSeconds(.2f);
        cursorMoving = false;
    }
}
