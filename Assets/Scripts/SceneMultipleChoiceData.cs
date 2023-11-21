using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSceneMultipleChoiceData", menuName = "Scene Multiple Choice Data", order = 52)]
public class SceneMultipleChoiceData : ScriptableObject
{

    public string question;

    public List<MultipleChoiceOption> choices = new List<MultipleChoiceOption>();

    private List<bool> choicesClicked;

    public void InitializeChoicesClicked()
    {
        choicesClicked = new List<bool>();
        choicesClicked.Clear();

        for (int i = 0; i < choices.Count; i++)
        {
            choicesClicked.Add(false);
        }

    }

    public bool CheckIfChoiceClicked(int index)
    {
        return choicesClicked[index];
    }
    public void RegisterChoiceClicked(int index)
    {
        choicesClicked[index] = true;
    }


    [Serializable]
    public struct MultipleChoiceOption
    {
        public string option;
        public bool isAnswer;
    }

}
