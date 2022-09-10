using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AntHill : MonoBehaviour
{
    public TextMeshPro displayUI;
    public int count;
    public int displayCount;

    private void Start()
    {
        count = 0;
        displayCount = 0;
        StartCoroutine(CountUpdater());
    }
    // Update is called once per frame
    private IEnumerator CountUpdater()
    {
        while (true)
        {
            if (displayCount < count)
            {
                displayCount++; //Increment the display score by 1
                displayUI.text = displayCount.ToString(); //Write it to the UI
            }
            yield return new WaitForSeconds(0.2f); // I used .2 secs but you can update it as fast as you want
        }
    }
}
