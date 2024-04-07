using System;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;
    
    [SerializeField] string csvFileName;
    
    Dictionary<int, Narration> narrationDic = new Dictionary<int, Narration>();
    
    public static bool isFinished = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            NarrationParser theParser = GetComponent<NarrationParser>();
            Narration[] narrations = theParser.Parse(csvFileName);

            for (int i = 0; i < narrations.Length; i++)
            {
                narrationDic.Add(i+1, narrations[i]);
            }
            isFinished = true;
        }
    }
    
    public Narration[] GetNarration(int start, int end)
    {
        List<Narration> narrations = new List<Narration>();
        for (int i = start; i <= end; i++)
            narrations.Add(narrationDic[i]);

        return narrations.ToArray();
    } 
}