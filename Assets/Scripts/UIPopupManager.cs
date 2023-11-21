using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UIPopupManager : MonoBehaviour
{



    private static UIPopupManager _instance;
    public static UIPopupManager Instance
    {
        get { return _instance; }
    }
    private object _instanceLock = new object();

    public Image popupImage;
    public GameObject popupImagePanel;
    public UIPopupMultipleChoice multipleChoicePanel;
    

    [Serializable]
    public struct ScenePopup
    {
        public SceneData scene;
        public Sprite popupImage;
        public SceneMultipleChoiceData multipleChoiceData;
        public bool multipleChoiceCompleted;
    }

    private List<int> activePopupIndexes = new List<int>();
    public ScenePopup[] scenePopups;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        ResetMultipleChoiceProgress();
    }

    /// <summary>
    /// Resets multiple choice progress
    /// </summary>
    void ResetMultipleChoiceProgress()
    {
        for (int i = 0; i < scenePopups.Length; i++)
        {
            if (scenePopups[i].multipleChoiceData != null)
                scenePopups[i].multipleChoiceData.InitializeChoicesClicked(); 
        }
    }

    /// <summary>
    /// Finds active popups,
    /// Removes old popups,
    /// Toggles relevant popups based on scenedata.
    /// Assign the popups to GuidedTourManager in the editor.
    /// All assigned popups (image, multiple choice quiz) will trigger when transitioning to the designated scenedata.
    /// </summary>
    /// <param name="sceneData"></param>
    public void UpdatePopup(SceneData sceneData)
    {
        // Locking instance to prevent crashing from spamming next/previous
        lock (_instanceLock)
        {
            // Get active popups
            GetActivePopups(sceneData);

            // Activate them
            ActivateActivePopups();


        }
    }

    /// <summary>
    /// Finds all popups for the Scenedata parameter
    /// </summary>
    /// <param name="sceneData"></param>
    /// <returns></returns>
    private void GetActivePopups(SceneData sceneData)
    {
        activePopupIndexes = new List<int>();

        // Grab all relevant popups and add them to a temporary list
        for (int i = 0; i < scenePopups.Length; i++)
        {
            if (scenePopups[i].scene == sceneData)
            {
                activePopupIndexes.Add(i);
            }
        }

    }

    /// <summary>
    /// Activate popups
    /// </summary>
    /// <param name="popupList"></param>
    private void ActivateActivePopups()
    {

        bool imageFound = false;
        bool quizFound = false;

        // Add relevant popups to the active popups list and activate them

        for (int i = 0; i < activePopupIndexes.Count; i++)
        {
            if (scenePopups[activePopupIndexes[i]].popupImage != null)
            {
                popupImage.sprite = scenePopups[activePopupIndexes[i]].popupImage;
                imageFound = true;
            }

            if (scenePopups[activePopupIndexes[i]].multipleChoiceData != null)
            {
                multipleChoicePanel.Initialize(scenePopups[activePopupIndexes[i]].multipleChoiceData);
                quizFound = true;
            }
        }

        popupImagePanel.gameObject.SetActive(imageFound);
        multipleChoicePanel.gameObject.SetActive(quizFound);
    }

    /// <summary> 
    /// Called from Multiple Choice panel when the correct answer has been clicked.
    /// </summary>
    /// <param name="data"></param>
    public void CompleteMultipleChoice(SceneMultipleChoiceData data)
    {

        for (int i = 0; i < scenePopups.Length; i++)
        {
            if (scenePopups[i].multipleChoiceData == data)
            {
                scenePopups[i].multipleChoiceCompleted = true;
                for (int j = 0; j < multipleChoicePanel.optionsParent.transform.childCount; j++)
                {
                    multipleChoicePanel.optionsParent.transform.GetChild(j).GetComponent<UIPopupMultipleChoiceOption>().UpdateColor();
                }
            }
        }

    }

    /// <summary>
    /// Has this multiple choice been answered correctly? Returns true or false
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool IsMultipleChoiceCompleted(SceneMultipleChoiceData data)
    {
        for (int i = 0; i < scenePopups.Length; i++)
        {
            if (scenePopups[i].multipleChoiceData == data)
            {
                Debug.Log("Multiple Choice Completed: " + scenePopups[i].multipleChoiceCompleted);
                
                return scenePopups[i].multipleChoiceCompleted;                
                
            }
        }

        return false;
    }

    /// <summary>
    /// Called from GuidedTourManager. Checks to see if multiple choice quiz (or all multiple choice quizzes) for the scene has been completed
    /// </summary>
    /// <returns></returns>
    public bool IsNextSceneAllowed()
    {
        bool allowed = true;

        for (int i = 0; i < activePopupIndexes.Count; i++)
        {
            if (scenePopups[activePopupIndexes[i]].multipleChoiceData != null)
                if (!scenePopups[activePopupIndexes[i]].multipleChoiceCompleted)
                    allowed = false;
        }

        return allowed;
    }

    /// <summary>
    /// If multiple choice panel is open, hide it.
    /// This is called when navigating away from a scene with a multiple choice panel, to prevent it from floating until animation stops.
    /// </summary>
    public void HideMultipleChoicePanel()
    {
        if (multipleChoicePanel.gameObject.activeInHierarchy)
        {
            multipleChoicePanel.gameObject.SetActive(false);
        }
    }

}
