using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    int currentPos = 0;
    [SerializeField] List<Image> images;
    public Image currentImage;

    public void NextImage()
    {
        currentPos++;
        if (currentPos == images.Count)
        {
            currentPos = 0;
        }

        currentImage = images[currentPos];
    }
    
    public void PreviousImage()
    {
        currentPos--;
        if (currentPos < 0)
        {
            currentPos = images.Count -1;
        }

        currentImage = images[currentPos];
    }
}
