using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UIPopupManager : MonoBehaviour {

    

    private static UIPopupManager _instance;
    public static UIPopupManager Instance
    {
        get { return _instance; }
    }
    private object _instanceLock = new object();
    
    public Image popupImage;
    public UIPopupMultipleChoice multipleChoicePanel;

    [Serializable]
    public struct ScenePopup
    {
        public SceneData scene;
        public Image popupImage;
        public SceneMultipleChoiceData multipleChoiceData;
    }

    private List<ScenePopup> activePopups = new List<ScenePopup>();
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
    }

    public void UpdatePopup(SceneData sceneData)
    {
        // Locking instance to prevent crashing from spamming next/previous
        lock (_instanceLock)
        {
            // Get active popups
            List<ScenePopup> popupList = GetActivePopups(sceneData);

            // Remove old popups
            RemoveUnusedPopups(popupList);

            // Activate active popups (don't do anything if they're already active)
            ActivateActivePopups(popupList);

            
        }
    }

    private List<ScenePopup> GetActivePopups(SceneData sceneData)
    {
        List<ScenePopup> popupList = new List<ScenePopup>();
        // Grab all relevant popups and add them to a temporary list
        for (int i = 0; i < scenePopups.Length; i++)
        {
            if (scenePopups[i].scene == sceneData)
            {
                popupList.Add(scenePopups[i]);
            }
        }

        return popupList;
    }

    private void RemoveUnusedPopups(List<ScenePopup> popupList)
    {
        List<ScenePopup> popupsToRemove = new List<ScenePopup>();
        // Remove any popups not in the list
        for (int i = 0; i < activePopups.Count; i++)
        {
            if (!popupList.Contains(activePopups[i]))
            {
                popupsToRemove.Add(activePopups[i]);
            }
        }

        for (int i = 0; i < popupsToRemove.Count; i++)
        {
            activePopups.Remove(popupsToRemove[i]);
        }
    }

    private void ActivateActivePopups(List<ScenePopup> popupList)
    {
        // Add relevant popups to the active popups list and activate them
        for (int i = 0; i < popupList.Count; i++)
        {
            if (!activePopups.Contains(popupList[i]))
            {
                activePopups.Add(popupList[i]);

                if (popupList[i].popupImage != null)
                    popupImage.sprite = popupList[i].popupImage.sprite;

                if (popupList[i].multipleChoiceData != null)
                    multipleChoicePanel.Initialize(popupList[i].multipleChoiceData);

            }
        }
    }

}
