using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    int currentPosition = 0;
    int previousPosition = 1;
    [SerializeField] List<Button> buttons;
    Player player0;
    Player player1;
    bool cursorMoving = false;
    [SerializeField] Sprite selectedImage;
    [SerializeField] Sprite normalImage;

    [SerializeField] bool isVertical = true;

    private void Start()
    {
        player0 = Rewired.ReInput.players.GetPlayer(0);
        player1 = Rewired.ReInput.players.GetPlayer(1);
        UpdateButtons();
        
    }
    private void Update()
    {
        if (isVertical)
        {
            if (!cursorMoving && player0.GetAxis("MovePawnVertical") <= -0.2 || player1.GetAxis("MovePawnVertical") <= -0.2)
            {
                cursorMoving = true;
                previousPosition = currentPosition;
                currentPosition++;
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
            else if (!cursorMoving && player0.GetAxis("MovePawnVertical") >= 0.2 || player1.GetAxis("MovePawnVertical") >= 0.2)
            {
                cursorMoving = true;
                previousPosition = currentPosition;
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
        }
        else
        {
            if (!cursorMoving && player0.GetAxis("MovePawnHorizontal") <= -0.2 || player1.GetAxis("MovePawnHorizontal") <= -0.2)
            {
                cursorMoving = true;
                previousPosition = currentPosition;
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
            else if (!cursorMoving && player0.GetAxis("MovePawnHorizontal") >= 0.2 || player1.GetAxis("MovePawnHorizontal") >= 0.2)
            {
                cursorMoving = true;
                previousPosition = currentPosition;
                currentPosition++;
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
        }

        if (player0.GetButtonDown("Select") || player1.GetButtonDown("Select"))
        {
            SelectedButtonPressed();
        }
    }

    public void UpdateButtons()
    {
        StartCoroutine(WaitForCursor());
        buttons[currentPosition].image.sprite = selectedImage;
        buttons[currentPosition].image.color = Color.white;
        buttons[previousPosition].image.sprite = normalImage;
        buttons[previousPosition].image.color = new Color(.8f, .8f, .8f);
        //cursor.position = buttons[currentPosition].GetComponent<RectTransform>().position;
    }

    public void SelectedButtonPressed()
    {
        buttons[currentPosition].onClick.Invoke();
    }


    IEnumerator WaitForCursor()
    {
        yield return new WaitForSecondsRealtime(.2f);
        cursorMoving = false;
        
    }
}
