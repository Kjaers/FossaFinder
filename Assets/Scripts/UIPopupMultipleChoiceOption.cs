using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPopupMultipleChoiceOption : MonoBehaviour
{
    public bool isAnswer;

    public void Initialize(string text, bool isAnswer)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = text;
        this.isAnswer = isAnswer;
    }
    public void Clicked()
    {
        transform.GetComponentInParent<UIPopupMultipleChoice>().ClickOption(this);
    }


}
