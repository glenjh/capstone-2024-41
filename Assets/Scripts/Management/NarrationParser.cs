using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrationParser : MonoBehaviour
{
    public Narration[] Parse(string fileName)
    {
        List<Narration> narrations = new List<Narration>();
        TextAsset csvData = Resources.Load<TextAsset>(fileName);
        
        string[] data = csvData.text.Split(new char[] {'\n'});

        for (int i = 1; i < data.Length;)
        {
            string[] row = data[i].Split(new char[] { ',' });
            
            Narration narration = new Narration();
            narration.name = row[1];
            List<string> contextList = new List<string>();

            do
            {
                contextList.Add(row[2]);
                if (++i < data.Length)
                    row = data[i].Split(new char[] { ',' });
                else
                    break;
            } while (row[0].ToString() == "");
            
            narration.Contexts = contextList.ToArray();
            
            narrations.Add(narration);
        }
        return narrations.ToArray();
    }
}