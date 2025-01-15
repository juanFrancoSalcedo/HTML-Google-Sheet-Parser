using UnityEngine;
using TMPro;

public class CardExample:MonoBehaviour
{
    [SerializeField] private TMP_Text textInseide;


    public void Show(string tex) 
    {
        textInseide.text = tex;
    }

}