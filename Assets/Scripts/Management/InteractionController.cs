using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("NarrationObject"))
        {
            NarrationManager.Instance.ShowNarration(other.gameObject.GetComponent<InteractionEvent>().GetNarration());
        }
    }
}