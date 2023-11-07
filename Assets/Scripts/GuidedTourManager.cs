﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.UI;

public enum TransitionType
{
    None,
    Forward,
    Backward,
    Outward,
    Inward
}

public class GuidedTourManager : MonoBehaviour
{
    private static GuidedTourManager _instance;
    public static GuidedTourManager Instance
    {
        get { return _instance; }
    }

    [Header("Show Scene Data Info (boolean)")]
    public bool displaySceneDataInfo = false;
    [Header("Narration Text UI")]
    public FadableUI currentlyNarratingUI;
    public Text sceneNameTextAsset;
    public Text animationNameTextAsset;
    public Text narrationTextAsset;
    public GameObject sceneDataCanvasGo;

    public GameObject head, headContainer, cameraRig, mainCamera;
    public Animator animator;
    private Animator oldAnimator;
    public Animator cameraAnimator;
    public AudioSource narrationSource;
    public GameObject miniSkull; // Included so it can be toggled via SceneData

    public GameObject instructions; // Included for toggling instructions when past Scene 2

    //the pulse script of the '[D] Next' button
    public PulsingAlphaEffect nextButtonPulsingEffect;

    public SceneData[] sceneDataArray;

    /// APP STATE EVENTS
    public delegate void DefaultStateHandler();
    public static event DefaultStateHandler DefaultState;

    public delegate void Initialize();
    public static event Initialize InitializeEvent;

    public delegate void VisitPrevious(SceneData sceneData);
    public static event VisitPrevious VisitPreviousEvent;

    public delegate void VisitNext(SceneData sceneData);
    public static event VisitNext VisitNextEvent;

    public delegate void ZoomIn(SceneData sceneData);
    public static event ZoomIn ZoomInEvent;

    public delegate void ZoomOut(SceneData sceneData);
    public static event ZoomOut ZoomOutEvent;

    public delegate void ZoomedOutHandler();
    public static event ZoomedOutHandler ZoomedOut;

    public delegate void DuringTransition();
    public static event DuringTransition DuringTransitionEvent;

    public delegate void Skip(SceneData sceneData);
    public static event Skip SkipEvent;

    public delegate void NarrationEnd();
    public static event NarrationEnd NarrationEndEvent;

    // VISUALS-SPECIFIC EVENTS (called in special cirumstances)

    public delegate void EnableBoundariesHandler(string[] names);
    public static event EnableBoundariesHandler EnableBoundaries;

    public delegate void SetRenderTextureHandler(string name);
    public static event SetRenderTextureHandler SetRenderTexture;

    public delegate void DisableRenderTextureHandler();
    public static event DisableRenderTextureHandler DisableRenderTexture;

    ActivityRecorder ar;


    Vector3 adjustedCameraPosition;
    int currentSceneNumber; // the current scene destination number
    static bool isDuringTransition;
    TransitionType currentTransitionType;
    string currentAnimationClipName;
    float currentAnimationClipLength;
    float distanceFromAdjustedCameraPositionThreshold;
    Coroutine afterAnimationCoroutine;
    bool afterAnimationCoroutineIsRunning;

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

    // Use this for initialization
    void Start()
    {
        currentSceneNumber = 1;
        isDuringTransition = false;
        currentTransitionType = TransitionType.None;
        distanceFromAdjustedCameraPositionThreshold = 0.2f;
        afterAnimationCoroutineIsRunning = false;
        oldAnimator = animator;

        InitializeEvent?.Invoke();

        //StartCoroutine(AdjustCameraRigAndUserHeight());
    }

    private void Update()
    {
        //can toggle the Scene Data Canvas on and off during run time using the editor
        sceneDataCanvasGo.SetActive(displaySceneDataInfo);

        //Manage narration UI object
        if (narrationSource != null)
        {
            if (narrationSource.isPlaying && narrationSource.clip != null)
            {
                //Should show the narration symbol
                //Conversely... the next button should be normal color
                currentlyNarratingUI.FadeIn();
                nextButtonPulsingEffect.SetPulsingEffect(false);
            }
            else
            {
                //do the opposite for both the narration symbol and next button
                //next button will change color
                currentlyNarratingUI.FadeOut();
                NarrationEndEvent?.Invoke();
                nextButtonPulsingEffect.SetPulsingEffect(true);
            }
        }
    }

    public void InjectRecorder(ActivityRecorder r)
    {
        ar = r;
    }

    // Defines the world position of the camera rig and the skull, after the position of the camera is set
    IEnumerator AdjustCameraRigAndUserHeight()
    {
        yield return new WaitForSeconds(.5f);

        cameraRig.transform.position = new Vector3(0, mainCamera.transform.position.y, .5f) - mainCamera.transform.position;
        headContainer.transform.position = new Vector3(0, mainCamera.transform.position.y, 0);

        adjustedCameraPosition = mainCamera.transform.position;
    }

    public bool showSceneDataInfo
    {
        get { return displaySceneDataInfo; }
        set { displaySceneDataInfo = value; }
    }

    // Returns the current scene number
    public int CurrentSceneNumber
    {
        get { return currentSceneNumber; }
        set { currentSceneNumber = value; }
    }

    public bool GetIsDuringTransition()
    {
        return isDuringTransition;
    }

    public TransitionType GetCurrentTransitionType()
    {
        return currentTransitionType;
    }

    public string CurrentAnimationClipName
    {
        get { return currentAnimationClipName; }
        set { currentAnimationClipName = value; }
    }

    //public void SetCurrentAnimationClipLength(float clipLength)
    //{
    //    currentAnimationClipLength = clipLength;
    //}

    // Adjusts all necessary variables for transitioning into the previous scene (the scene with the smaller scene number). TransitionToAnotherScene() will handle the actual animation
    public void VisitPreviousScene()
    {
        Debug.Log("Before decrement: " + currentSceneNumber);
        if (currentSceneNumber > 1)
        {
            currentSceneNumber -= 1;
            isDuringTransition = true;
            currentTransitionType = TransitionType.Backward;
            currentAnimationClipName = sceneDataArray[currentSceneNumber - 1].backwardAnimationClipName;
            currentAnimationClipLength = sceneDataArray[currentSceneNumber - 1].backwardAnimationClipLength;


            if (sceneDataArray[currentSceneNumber - 1] is ExteriorSceneData)
            {
                if (!(sceneDataArray[currentSceneNumber] is ExteriorSceneData))
                {
                    animator = cameraAnimator;
                    ExteriorSceneData currentExteriorSceneData = (ExteriorSceneData)sceneDataArray[currentSceneNumber - 1];
                    SetRenderTexture?.Invoke(currentExteriorSceneData.renderTexture);
                }

            }
            else
            {
                if (sceneDataArray[currentSceneNumber] is ExteriorSceneData)
                {
                    // sceneDataArray[currentSceneNumber - 1].AssignAnimatorAndRuntimeController(this);
                    animator = oldAnimator;
                    DisableRenderTexture?.Invoke();
                }
            }

            VisitPreviousEvent?.Invoke(sceneDataArray[currentSceneNumber - 1]);

            PlayTransition();
            Debug.Log(sceneDataArray[currentSceneNumber - 1].name);

            if (ar != null)
            {
                ar.QueueMessage("VisitPreviousScene");
            }

            bool isNarrationPresent = sceneDataArray[currentSceneNumber - 1].narration != null;

            // Stop current narration
            StopNarration();

            // If the SceneData specifies the MiniSkull being on or off, make that happen.
            if (sceneDataArray[currentSceneNumber - 1].showMiniSkull != SceneData.enableMiniSkull.Ignore)
            {
                if (sceneDataArray[currentSceneNumber - 1].showMiniSkull == SceneData.enableMiniSkull.Disable)
                {
                    miniSkull.SetActive(false);
                }

                if (sceneDataArray[currentSceneNumber - 1].showMiniSkull == SceneData.enableMiniSkull.Enable)
                {
                    miniSkull.SetActive(true);
                }

            }

            // Kludge: Manually turn on instructions for now when entering scene 1, 2
            if (currentSceneNumber < 3)
            {
                instructions.active = true;
            }

            //Display SceneDataInfo
            DisplaySceneDataInfo(sceneDataArray[currentSceneNumber - 1].name, sceneDataArray[currentSceneNumber - 1].backwardAnimationClipName, isNarrationPresent ? sceneDataArray[currentSceneNumber - 1].narration.name : "");
        }
    }

    /// <summary>
    ///  Adjusts all necessary variables for transitioning into the previous scene (the scene with the smaller scene number). TransitionToAnotherScene() will handle the actual animation
    /// previousScene: define the previous scene.  If it was not provided, we will assume it follows a liner track (ie: look at the last scenes from the current track)
    /// if previousScene was provided, we need to explicity override the default value
    /// </summary>
    public float VisitNextScene(int inputPreviousScene)
    {
        Debug.Log("Before increment: " + currentSceneNumber);
        if (currentSceneNumber < sceneDataArray.Length)
        {
            currentSceneNumber += 1;
            isDuringTransition = true;
            currentTransitionType = TransitionType.Forward;
            currentAnimationClipName = sceneDataArray[currentSceneNumber - 1].forwardAnimationClipName;
            currentAnimationClipLength = sceneDataArray[currentSceneNumber - 1].forwardAnimationClipLength;

            int previousScene = currentSceneNumber - 2;
            if (inputPreviousScene != -1)
            {
                previousScene = inputPreviousScene - 1;
            }

            //if the 'next' scene is a exterior scene data
            if (sceneDataArray[currentSceneNumber - 1] is ExteriorSceneData)
            {
                //...and the previous one is a normal scene data, then update the camera and render texture
                if (sceneDataArray[previousScene] is SceneData)
                {
                    animator = cameraAnimator;
                    ExteriorSceneData currentExteriorSceneData = (ExteriorSceneData)sceneDataArray[currentSceneNumber - 1];
                    SetRenderTexture?.Invoke(currentExteriorSceneData.renderTexture);
                }
            }
            else
            {
                //going from exterior scene to a normal scene
                if (sceneDataArray[previousScene] is ExteriorSceneData && sceneDataArray[currentSceneNumber - 1] is SceneData)
                {
                    animator = oldAnimator;
                    DisableRenderTexture?.Invoke();
                }
            }

            VisitNextEvent?.Invoke(sceneDataArray[currentSceneNumber - 1]);
            PlayTransition();
            Debug.Log(sceneDataArray[currentSceneNumber - 1].name);
            if (ar != null)
            {
                ar.QueueMessage("VisitNextScene");
            }

            // If the SceneData specifies the MiniSkull being on or off, make that happen.
            if (sceneDataArray[currentSceneNumber - 1].showMiniSkull != SceneData.enableMiniSkull.Ignore)
            {
                if (sceneDataArray[currentSceneNumber - 1].showMiniSkull == SceneData.enableMiniSkull.Disable)
                {
                    miniSkull.SetActive(false);
                }

                if (sceneDataArray[currentSceneNumber - 1].showMiniSkull == SceneData.enableMiniSkull.Enable)
                {
                    miniSkull.SetActive(true);
                }

            }

            bool isNarrationPresent = sceneDataArray[currentSceneNumber - 1].narration != null;

            StopNarration();

            if (isNarrationPresent)
            {
                Debug.Log("Playing Narration Clip: " + sceneDataArray[currentSceneNumber - 1].narration.name);
                narrationSource.clip = sceneDataArray[currentSceneNumber - 1].narration;
                narrationSource.PlayOneShot(narrationSource.clip);
            }


            // Kludge: Manually turn on instructions for now when entering scene 1
            if (currentSceneNumber > 2)
            {
                instructions.active = false;
            }

            //Display SceneDataInfo
            DisplaySceneDataInfo(sceneDataArray[currentSceneNumber - 1].name, sceneDataArray[currentSceneNumber - 1].forwardAnimationClipName, isNarrationPresent ? sceneDataArray[currentSceneNumber - 1].narration.name : "");


            // Handle popups
            UIPopupManager.Instance.UpdatePopup(sceneDataArray[currentSceneNumber - 1]);

            return currentAnimationClipLength;
        }
        return 0f;
    }

    // Maintains all necessary variables for transitioning into the next scene (the scene with the greater scene number). PlayTransition() will handle the actual animation
    public float VisitNextScene()
    {
        return VisitNextScene(-1);
    }

    private void StopNarration()
    {
        //Stop Narration
        if (narrationSource != null)
        {
            narrationSource.Stop();
        }
        NarrationEndEvent?.Invoke();
    }

    public void DisplaySceneDataInfo(string sceneName, string animationName, string narrationName)
    {
        //show current scene + animation + narration name
        sceneNameTextAsset.text = sceneName;
        animationNameTextAsset.text = animationName;
        narrationTextAsset.text = narrationName;
    }

    public void ZoomInToCurrentScene()
    {
        isDuringTransition = true;
        currentTransitionType = TransitionType.Inward;
        currentAnimationClipName = sceneDataArray[currentSceneNumber - 1].ZoomInAnimationClipName;
        currentAnimationClipLength = sceneDataArray[currentSceneNumber - 1].ZoomInAnimationClipLength;


        ZoomInEvent?.Invoke(sceneDataArray[currentSceneNumber - 1]);

        PlayTransition();
        if (ar != null)
        {
            ar.QueueMessage("ZoomInToCurrentScene");
        }
    }

    public void ZoomOutFromCurrentScene()
    {
        isDuringTransition = true;
        currentTransitionType = TransitionType.Outward;
        currentAnimationClipName = sceneDataArray[currentSceneNumber - 1].ZoomOutAnimationClipName;
        currentAnimationClipLength = sceneDataArray[currentSceneNumber - 1].ZoomOutAnimationClipLength;


        ZoomOutEvent?.Invoke(sceneDataArray[currentSceneNumber - 1]);

        PlayTransition();
        if (ar != null)
        {
            ar.QueueMessage("ZoomOutFromCurrentScene");
        }
    }

    // Checks whether the skull needs to be adjusted first. Then, plays the appropriate transition animation clip.
    void PlayTransition()
    {
        Debug.Log("Playing Animation: " + CurrentAnimationClipName);
        //AdjustSkullPositionIfPastThreshold();

        if (!string.IsNullOrEmpty(currentAnimationClipName))
        {
            animator.Play(currentAnimationClipName, 0, 0f);
            DuringTransitionEvent?.Invoke();
        }

        afterAnimationCoroutine = StartCoroutine(AfterAnimation());
    }

    void AdjustSkullPositionIfPastThreshold()
    {
        Vector3 currentCameraPosition = mainCamera.transform.position;
        if (Vector3.Distance(currentCameraPosition, adjustedCameraPosition) > distanceFromAdjustedCameraPositionThreshold)
        {
            Vector3 offset = new Vector3(currentCameraPosition.x - adjustedCameraPosition.x, currentCameraPosition.y - adjustedCameraPosition.y, currentCameraPosition.z - adjustedCameraPosition.z);
            headContainer.transform.position += offset;
            adjustedCameraPosition = mainCamera.transform.position;
        }
    }

    IEnumerator AfterAnimation()
    {
        afterAnimationCoroutineIsRunning = true;
        yield return new WaitForSeconds(currentAnimationClipLength);
        isDuringTransition = false;
        if (currentTransitionType == TransitionType.Outward)
        {
            EnableBoundaries?.Invoke(sceneDataArray[currentSceneNumber - 1].boundaries);
        }
        ChangeButtonStatesAfterAnimationCompleted();
        currentTransitionType = TransitionType.None;
        currentAnimationClipName = "";
        currentAnimationClipLength = 0;
        afterAnimationCoroutineIsRunning = false;
    }

    void ChangeButtonStatesAfterAnimationCompleted()
    {
        if (currentTransitionType == TransitionType.Forward || currentTransitionType == TransitionType.Backward || currentTransitionType == TransitionType.Inward)
        {
            DefaultState?.Invoke();
        }
        else if (currentTransitionType == TransitionType.Outward)
        {
            ZoomedOut?.Invoke();
        }
    }

    public void SkipTransition()
    {
        if (afterAnimationCoroutineIsRunning)
        {
            StopCoroutine(afterAnimationCoroutine); /// you need this because you don't want this effect to take place unintentionally
            afterAnimationCoroutineIsRunning = false;
        }
        animator.Play(currentAnimationClipName, -1, 1);
        isDuringTransition = false;
        currentTransitionType = TransitionType.None;
        currentAnimationClipName = "";
        currentAnimationClipLength = 0;
        DefaultState?.Invoke();

        StopNarration();

        SkipEvent?.Invoke(sceneDataArray[currentSceneNumber - 1]);
        if (ar != null)
        {
            ar.QueueMessage("SkipTransition");
        }
    }

    public void VisitOrSkipNextScene()
    {
        if (GetIsDuringTransition())
        {
            if (GetCurrentTransitionType() == TransitionType.Forward)
            {
                SkipTransition();
            }
        }
        else
        {
            VisitNextScene();
        }
    }

    public void VisitOrSkipPreviousScene()
    {
        if (GetIsDuringTransition())
        {
            if (GetCurrentTransitionType() == TransitionType.Backward)
            {
                SkipTransition();
            }
        }
        else
        {
            VisitPreviousScene();
        }
    }

    public void ToggleSceneDataInfo()
    {
        showSceneDataInfo = !showSceneDataInfo;
    }
}
