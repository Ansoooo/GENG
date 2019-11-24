using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public GameObject[] animationsList;
    int currentAnim;
    bool direction;

    public void switchAnimation(int targetAnim)
    {
        if (targetAnim != currentAnim) // so animations dont stall if we call the same one
        {
            animationsList[currentAnim].SetActive(false);
            animationsList[targetAnim].SetActive(true);
            currentAnim = targetAnim;
        }
    }

    public void turnAround(bool _direction) // false = right, true = left
    {
        direction = _direction;
    }

    void Start()
    {
        currentAnim = 0;
        direction = false;
        animationsList[0].SetActive(true);
    }

    
    void Update()
    {
        animationsList[currentAnim].GetComponent<SpriteRenderer>().flipX = direction;
    }
}
