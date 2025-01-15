using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardGenerator : MonoBehaviour,ITableInjectable
{
    [SerializeField] private CardExample prototype;
    [SerializeField] GridLayoutGroup glg;

    public void Configure(List<List<string>> tableData) 
    {
        glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        glg.constraintCount = tableData[1].Count;

        foreach (var item in tableData)
        {
            foreach (var cardContent in item)
            {
                PublishCard(cardContent);
            }
        }
    }

    public void PublishCard(string strValue)
    {
        var clone = Instantiate(prototype,glg.transform);
        clone.Show(strValue);
    }
}
