using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RandomizeButtonBehaviour : MonoBehaviour, IPointerDownHandler
{

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        ModelScript.Randomize();
    }
}
