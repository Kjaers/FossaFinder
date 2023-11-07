using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSceneMultipleChoiceData", menuName = "Scene Multiple Choice Data", order = 52)]
public class SceneMultipleChoiceData : ScriptableObject {

    public List<MultipleChoiceEntry> choices = new List<MultipleChoiceEntry>();
    [Serializable]
    public struct MultipleChoiceEntry
    {
        public string question;
        public bool isAnswer;
    }

}
