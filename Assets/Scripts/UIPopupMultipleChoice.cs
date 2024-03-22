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

        // If no choices are present, popup is just used for information. In this case, we can assume it's for results
        if (data.choices.Count == 0)
        {
            questionText.text = "Congratulations!\n\nYour final score is:\n\nCorrect: " + UIPopupManager.Instance.correctAnswers + "\n\n" + "Incorrect: " + UIPopupManager.Instance.incorrectAnswers;
        }
        else
        {
            for (int i = 0; i < data.choices.Count; i++)
            {
                GameObject newOption = Instantiate(optionPrefab, optionsParent.transform);

                newOption.GetComponent<UIPopupMultipleChoiceOption>().Initialize(data, i);

            }
        }


    }

    void OnEnable()
    {
        Resize();
    }

    public void Resize()
    {
        Debug.Log("Resizing for question: " + questionText.text);
        if (data.question.Length > 180)
        {
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(380, transform.GetComponent<RectTransform>().sizeDelta.y);
        }
        else
        {
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(300, transform.GetComponent<RectTransform>().sizeDelta.y);
        }
        //if (questionText.gameObject.GetComponent<RectTransform>().sizeDelta.y > 150)
        //{
        //    transform.GetComponent<RectTransform>().sizeDelta = new Vector2(380, transform.GetComponent<RectTransform>().sizeDelta.y);
        //}
        //else
        //{
        //    transform.GetComponent<RectTransform>().sizeDelta = new Vector2(300, transform.GetComponent<RectTransform>().sizeDelta.y);
        //}
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
