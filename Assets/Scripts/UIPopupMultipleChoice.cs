using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPopupMultipleChoice : MonoBehaviour
{

    public GameObject optionPrefab;
    public GameObject optionsParent;
    public TextMeshProUGUI questionText;
    SceneMultipleChoiceData data;

    public void Initialize(SceneMultipleChoiceData multipleChoiceData)
    {
        data = multipleChoiceData;
        questionText.text = data.question;

        ClearOptions();        

        for (int i = 0; i < data.choices.Count; i++)
        {           
            GameObject newOption = Instantiate(optionPrefab, optionsParent.transform);

            newOption.GetComponent<UIPopupMultipleChoiceOption>().Initialize(data, i);

        }

    }

    //public void ClickOption(UIPopupMultipleChoiceOption option)
    //{
    //    option.wasClicked = true;
    //    if (option.isAnswer)
    //    {
    //        Debug.Log("Correct answer clicked!");
    //        UIPopupManager.Instance.CompleteMultipleChoice(data);
    //    }
    //    else
    //    {
    //        Debug.Log("Incorrect answer clicked!");
    //    }
    //}

    private void ClearOptions()
    {
        foreach (Transform child in optionsParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }


}
