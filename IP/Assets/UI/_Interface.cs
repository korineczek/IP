using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class _Interface : Singleton<_Interface>
{

    

    private _Filters f;
    private _Manager manager;

    public RectTransform FilterButton;

	// Use this for initialization
	void Awake ()
	{
	    f = _Filters.Instance;
	    manager = _Manager.Instance;

	   // generate UI
	    for (int i = 0; i < manager.FilterNames.Count; i++)
	    {
            RectTransform newButton = Instantiate(FilterButton, new Vector3(0 + FilterButton.rect.width / 2, Screen.height - FilterButton.rect.height / 2 - FilterButton.rect.height * i), Quaternion.Euler(Vector3.zero)) as RectTransform;


	        newButton.GetComponent<FilterButton>().Index = i;
	        newButton.GetChild(0).GetComponent<Text>().text = manager.FilterNames[i];
            newButton.SetParent(this.transform);
	    }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
