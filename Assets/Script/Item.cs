using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBadge", menuName = "Badge")]
public class Item : ScriptableObject
{
    public string itemName;       // ���O
    public Sprite icon;           // �A�C�R��
    public List<string> tags;     // �^�O

    
    /*public void GenerateRandomTags(List<string> allPossibleTags,int num)
    {
        tags = new List<string>();
        while (tags.Count < num)
        {
            string randomTag = allPossibleTags[Random.Range(0, allPossibleTags.Count)];
            if (!tags.Contains(randomTag))
                tags.Add(randomTag);
        }
    }*/
}