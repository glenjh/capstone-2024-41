using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TalkTest : MonoBehaviour
{
    public Collider2D col;
    public GameObject talkUI;
    public TextMeshProUGUI narrationText;
    
    public bool isTalk = false;
    public bool isProduction = false;
    public int talkIndex = 0;
    public int talkCount = 0;
    public bool isFinished = false;
    public bool isSkiped = false;
    
    [SerializeField] private NarrationEvent narration;
    Coroutine lastRoutine = null;
    
    void Start()
    {
        col = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("InterCol")&&!isProduction)
        {
            talkUI.SetActive(true);
            other.GetComponent<InteractionController>().AddInteraction(SkipNarration);
            other.GetComponent<InteractionController>().AddInteraction(NextNarration);
            other.GetComponent<InteractionController>().AddInteraction(ShowNarration);
            ReadyNarration();
        }
    }
    public void GetNarration()
    {
        narration.narrations = DatabaseManager.Instance.GetNarration((int)narration.line.x, (int)narration.line.y);
    }
    
    private void ShowNarration()
    {
        if (isTalk)
            return;
        
        if(Input.GetKeyDown(KeyCode.F))
        {
            isTalk = true;
            talkIndex = 0;
            isFinished = false;
            StopCoroutine(lastRoutine);
            narrationText.text = "";
            lastRoutine = StartCoroutine(ShowNarrationCoroutine());
        }
    }
    public void ShowNarration(string indexStr)
    {
        if (talkUI.activeSelf==false)
            talkUI.SetActive(true);
        
        int start = 0;
        foreach (var c in indexStr)
        {
            if (c == ',')
                break;
            start = start * 10 + (c - '0');
        }
        
        int end = 0;

        for (int i = indexStr.Length - 1; i >= 0; i--)
        {
            if (indexStr[i] == ',')
                break;
            end = end * 10 + (indexStr[i] - '0');
        }
        

        narration.narrations = DatabaseManager.Instance.GetNarration(start, end);
        lastRoutine = StartCoroutine(ShowProduction());
    }
    
    private void SkipNarration()
    {
        if (isTalk && !isFinished)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                StopCoroutine(lastRoutine);
                narrationText.text = narration.narrations[talkIndex].Contexts[talkCount];

                isFinished = true;
                isSkiped = true;
            }
        }
    }
    
    private void NextNarration()
    {
        if (isSkiped)
        {
            isSkiped = false;
            return;
        }
        
        if (isTalk&&isFinished)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if(talkCount < narration.narrations[talkIndex].Contexts.Length - 1)
                {
                    talkCount++;
                    lastRoutine = StartCoroutine(ShowNarrationCoroutine());
                    isFinished = false;
                }
                else
                {
                    if (talkIndex < narration.narrations.Length - 1)
                    {
                        talkIndex++;
                        talkCount = 0;
                        lastRoutine = StartCoroutine(ShowNarrationCoroutine());
                        isFinished = false;
                    }
                    else
                    {
                        StopAllCoroutines();
                        talkUI.SetActive(false);
                        isTalk = false;
                        talkIndex = 0;
                        talkCount = 0;
                    }
                }
            }
        }
    }

    IEnumerator ShowNarrationCoroutine()
    {
        narrationText.text = "";
        for (int j = 0; j < narration.narrations[talkIndex].Contexts[talkCount].Length; j++)
        {
            narrationText.text += narration.narrations[talkIndex].Contexts[talkCount][j];
            yield return new WaitForSeconds(0.1f);
        }

        isFinished = true;
    }

    IEnumerator ShowProduction()
    {
        for (int i = 0; i < narration.narrations.Length; i++)
        {
            for (int j = 0; j < narration.narrations[i].Contexts.Length; j++)
            {
                narrationText.text = "";
                for (int k = 0; k < narration.narrations[i].Contexts[j].Length; k++)
                {
                    narrationText.text += narration.narrations[i].Contexts[j][k];
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
        talkUI.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("InterCol"))
        {
            talkUI.SetActive(false);
            other.GetComponent<InteractionController>().Interaction -= ShowNarration;
            other.GetComponent<InteractionController>().Interaction -= SkipNarration;
            other.GetComponent<InteractionController>().Interaction -= NextNarration;
            StopAllCoroutines();
            isTalk = false;
        }
    }
    
    // 대기중 텍스트 출력
    private void ReadyNarration()
    {
        if(isTalk) return;

        lastRoutine = StartCoroutine(ReadyText());
    } 
    
    // ... 텍스트 출력
    IEnumerator ReadyText()
    {
        while (!isTalk)
        {
            narrationText.text = "";
            for (int i = 0; i < 3; i++)
            {
                narrationText.text += '.';
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
