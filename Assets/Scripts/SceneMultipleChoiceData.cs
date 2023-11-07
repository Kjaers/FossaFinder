using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSceneMultipleChoiceData", menuName = "Scene Multiple Choice Data", order = 52)]
public class SceneMultipleChoiceData : ScriptableObject {

    public string question;

    public List<MultipleChoiceOption> choices = new List<MultipleChoiceOption>();
    [Serializable]
    public struct MultipleChoiceOption
    {
        public string option;
        public bool isAnswer;
    }

}
