using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TutoUI : MonoBehaviour
{
   public GameObject tutoObj;

   private void Start()
   {
      tutoObj.transform.localScale = new Vector3(0, 0, 0);
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.gameObject.CompareTag("Player"))
      {
         tutoObj.transform.DOScale(1f, 0.7f).SetEase(Ease.OutQuart);
      }
   }

   private void OnTriggerExit2D(Collider2D other)
   {
      if (other.gameObject.CompareTag("Player"))
      {
         tutoObj.transform.DOScale(0f, 0.7f).SetEase(Ease.OutQuart);
      }
   }
}
