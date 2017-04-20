using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIButton : Button
{
    public delegate void OnClicked(UIButton button);
    public OnClicked onClicked;

    protected override void Awake()
    {
        base.Awake();

        onClick.AddListener(delegate()
        {
            if (onClicked != null)
            {
                onClicked(this);
            }
        });
    }

    // Use this for initialization

    // Update is called once per frame

}
