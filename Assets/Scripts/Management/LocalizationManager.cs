using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    public List<DialogueData> dialogueDataList; // JSON 파일에서 읽어온 대본 데이터 리스트
    private Dictionary<string, Dictionary<string, string>> localizedDialogues; // 언어별 대본을 저장하는 딕셔너리

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadDialogueData(); // 대본 데이터 로드
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadDialogueData()
    {
        // JSON 파일에서 대본 데이터 로드하여 dialogueDataList에 저장하는 로직
        // 예시: dialogueDataList = LoadDialogueDataFromJSON();
        // 실제 구현은 JSON 파일을 읽어와서 데이터를 파싱하도록 합니다.
        
        // 각 언어별 대본을 localizedDialogues에 저장
        localizedDialogues = new Dictionary<string, Dictionary<string, string>>();
        foreach (DialogueData dialogueData in dialogueDataList)
        {
            localizedDialogues[dialogueData.id] = dialogueData.localizedTexts;
        }
    }

    public string GetLocalizedDialogue(string dialogueId, string language)
    {
        if (localizedDialogues.ContainsKey(dialogueId))
        {
            Dictionary<string, string> localizedTexts = localizedDialogues[dialogueId];
            if (localizedTexts.ContainsKey(language))
            {
                return localizedTexts[language];
            }
        }

        return "Missing Dialogue"; // 대본이 없을 경우 기본 문구 반환
    }
}

[System.Serializable]
public class DialogueData
{
    public string id;
    public Dictionary<string, string> localizedTexts; // 각 언어별 대본을 저장하는 딕셔너리
}