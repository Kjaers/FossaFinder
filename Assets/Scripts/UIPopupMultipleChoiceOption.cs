using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupMultipleChoiceOption : MonoBehaviour
{
    SceneMultipleChoiceData data;
    int choiceIndex;
    //SceneMultipleChoiceData.MultipleChoiceOption source;
    //public bool isAnswer;
    //public bool wasClicked;
    ColorBlock colors;

    public void Initialize(SceneMultipleChoiceData data, int choiceIndex)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = data.choices[choiceIndex].option;
        this.data = data;
        this.choiceIndex = choiceIndex;
        //source = option;
        //this.isAnswer = option.isAnswer;
        //wasClicked = option.wasClicked;
        UpdateColor();

    }
    public void Clicked()
    {
        if (!data.CheckIfChoiceClicked(choiceIndex))
        {
            //transform.GetComponentInParent<UIPopupMultipleChoice>().ClickOption(this);
            data.RegisterChoiceClicked(choiceIndex);
            //UpdateColor();
            if (data.choices[choiceIndex].isAnswer)
            {
                Debug.Log("Correct answer clicked!");
                UIPopupManager.Instance.CompleteMultipleChoice(data);
            }
            else
            {
                Debug.Log("Incorrect answer clicked!");
            }

            UpdateColor();
        }
    }

    public void UpdateColor()
    {
        colors = GetComponent<Button>().colors;
        if (data.CheckIfChoiceClicked(choiceIndex) || UIPopupManager.Instance.IsMultipleChoiceCompleted(data))
        {
            if (data.choices[choiceIndex].isAnswer)
            {
                Color newColor = new Color(0.2156863f, 0.5960785f, 0.2601613f);
                colors.normalColor = newColor;
                colors.disabledColor = newColor;
            }
            else
            {
                if (data.CheckIfChoiceClicked(choiceIndex))
                {
                    Color newColor = new Color(0.5960785f, 0.2156863f, 0.2495756f);
                    colors.normalColor = newColor;
                    colors.disabledColor = newColor;
                }
                else
                {
                    // Keep normal color
                    Color newColor = new Color(0.2158686f, 0.3568676f, 0.5943396f);
                    colors.disabledColor = newColor;
                }
            }

            GetComponent<Button>().interactable = false;
        }

        GetComponent<Button>().colors = colors;
    }

    public void OnHover()
    {
        if (data.choices[choiceIndex].option.Substring(0, 7) == "Option " && data.choices[choiceIndex].option.Length == 8)
        {
            UIPopupManager.Instance.syringe.MoveToPositionIndex(choiceIndex);
        }
    }


}
