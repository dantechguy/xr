using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectFocusBottom : MonoBehaviour
{
    private ScrollRect scrollRect_;
    
    private void Start()
    {
        scrollRect_ = GetComponent<ScrollRect>();
        ScrollToBot();
    }

    private void OnEnable()
    {
        if (scrollRect_ != null)
            ScrollToBot();
    }


    public void ScrollToBot()
    {
        scrollRect_.verticalNormalizedPosition = 0;
    }
}
