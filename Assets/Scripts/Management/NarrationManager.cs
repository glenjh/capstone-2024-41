using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NarrationManager : MonoBehaviour
{
    public static NarrationManager Instance { get; private set; }
    public TextMeshProUGUI narrationText;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject.transform.root);
            
            narrationText = GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Destroy(this);
        }
    }
    
    /// <summary>
    /// show narration texts
    /// </summary>
    public void ShowNarration(Narration[] narrations)
    {
        //나레이션 텍스트를 1초마다 한줄씩 출력
        StartCoroutine(ShowNarrationCoroutine(narrations));
    }
    
    IEnumerator ShowNarrationCoroutine(Narration[] narrations)
    {
        for (int i = 0; i < narrations.Length; i++)
        {
            for (int j = 0; j < narrations[i].Contexts.Length; j++)
            {
                narrationText.text = narrations[i].Contexts[j];
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
