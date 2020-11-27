using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using AntiLagMod.settings.views;
using AntiLagMod.settings;
using UnityEngine.Events;
using BS_Utils.Utilities;

namespace AntiLagMod
    
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class AntiLagModController : MonoBehaviour
    {
        #region global variables
        public static AntiLagModController Instance { get; private set; }

        public bool criticalError;

        private bool modEnabled;
        private float frameThreshold;
        private bool frameDropDetection;
        private float waitThenActiveTime;

        private bool isLevel;
        private bool test = true;

        private int frameRate;
        private int frameCounter = 0;
        private float timeCounter = 0.0f;
        private float lastFrameRate;
        private float refreshRate = 0.25f;

        private bool activePause = false;
        private bool waitThenActiveFireOnce;

        // frame drop stuff here ^^^
        // tracking issues here vvv

        private bool trackingActiveFireOnce;

        private bool driftDetected;
        private bool trackingLossDetected;

        private bool driftDetection;
        private float driftThreshold;

        public Saber rSaber;
        public Saber lSaber;
        private Vector3 rSaberPos;
        private Vector3 lSaberPos;
        private Vector3 prevRSaberPos;
        private Vector3 prevLSaberPos;
        private int framesSinceLastSaberPosUpdate = 0;

        public PlayerHeightDetector PlayerHeightDetector;

        public static PauseController PauseController;

        
        #endregion

        #region Monobehaviour Messages
        private void Awake()
        {
            // For this particular MonoBehaviour, we only want one instance to exist at any time, so store a reference to it in a static property
            //   and destroy any that are created while one already exists.
            if (Instance != null)
            {
                Plugin.Log?.Warn($"Instance of {GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this); // Don't destroy this object on scene changes
            Instance = this;
            Plugin.Log?.Debug($"{name}: Awake()");

        }
        private void Start()
        {
            if (!criticalError)
                Refresh();
            waitThenActiveFireOnce = true;
            trackingActiveFireOnce = true;
            FindSabers();
        }
        private void Update()
        {
            CheckFrameRate();
            CheckEvents();

            if (isLevel && modEnabled)
            {
                if (waitThenActiveFireOnce)
                {
                    StartCoroutine(WaitThenActive());
                    waitThenActiveFireOnce = false;
                }
                if (!activePause)
                {
                    PauseController.didResumeEvent += OnLevelResume;
                }

                #region frame drop detection
                if (frameDropDetection)
                {
                    //Plugin.Log.Debug("FR: " + frameRate);
                    if (activePause && (frameRate < frameThreshold))
                    {
                        Pause();
                        Plugin.Log.Warn("FPS DROP DETECTED");
                        Plugin.Log.Debug("FPS 1 frame before drop: " + frameRate);
                    }
                }
                #endregion

                #region tracking issues detection

                if (driftDetection)
                {
                    string whichController = "(error getting which controller)";

                    CheckSaberPos("last");
                    if(framesSinceLastSaberPosUpdate == 1)
                    {
                        framesSinceLastSaberPosUpdate = 0;
                        CheckSaberPos("first");
                    }
                    framesSinceLastSaberPosUpdate++;
                    Plugin.Log.Debug("x" + rSaberPos.x + " y" + rSaberPos.y + " z" + rSaberPos.z); // log saber position

                    //tracking loss
                    if (rSaberPos.x == prevRSaberPos.x && rSaberPos.y == prevRSaberPos.y && rSaberPos.z == prevRSaberPos.z)
                    {
                        trackingLossDetected = true;
                        whichController = "left";
                    }
                    if (lSaberPos.x == prevLSaberPos.x && lSaberPos.y == prevLSaberPos.y && lSaberPos.z == prevLSaberPos.z)
                    {
                        trackingLossDetected = true;
                        whichController = "right";
                    }
                    if (activePause && trackingLossDetected)
                    {
                        Plugin.Log.Debug("Tracking issues detected with " + whichController + " controller.");
                        Pause();
                    }

                }

                #endregion
            }
        }
        private void FixedUpdate()
        {
            if (isLevel && modEnabled)
            {
                //
            }
        }
        private void OnEnable()
        {
            isLevel = false;
        }
        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            if (Instance == this)
                Instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.

        }
        #endregion

        private void CheckSaberPos(string firstOrLast)
        {
            if (firstOrLast == "first")
            {
                try
                {
                    rSaberPos = rSaber.handlePos;
                    lSaberPos = lSaber.handlePos;
                }
                catch (Exception exception)
                {
                    CriticalErrorHandler(true, 165, exception);
                }
            }
            if (firstOrLast == "last")
            {
                try
                {
                    prevRSaberPos = rSaber.handlePos;
                    prevRSaberPos = lSaber.handlePos;
                }
                catch (Exception exception)
                {
                    CriticalErrorHandler(true, 177, exception);
                }
            }


        }
        private void Pause()
        {
            activePause = false;
            PauseController.Pause();
        }
        private void FindSabers()
        {
            //int saberTypeArrayLength = Resources.FindObjectsOfTypeAll<Saber>().Length;
            rSaber = Resources.FindObjectsOfTypeAll<Saber>().ElementAt(0); // i hope this is right
            lSaber = Resources.FindObjectsOfTypeAll<Saber>().ElementAt(1);
            //Plugin.Log.Debug("There are " + saberTypeArrayLength + " instances of type Saber");
        }
        public static void Refresh() // refresh the class variables to equal the property variables
        {
            // I farted really hard when I wrote this
            Instance.modEnabled = Configuration.ModEnabled;
            Instance.frameDropDetection = Configuration.FrameDropDetectionEnabled;
            Instance.frameThreshold = Configuration.FrameThreshold;
            Instance.waitThenActiveTime = Configuration.WaitThenActive;

            Instance.driftDetection = Configuration.TrackingErrorDetectionEnabled;
            Instance.driftThreshold = Configuration.DriftThreshold;
        }
        private void CheckFrameRate()
        {
            if (frameCounter < refreshRate)
            {
                timeCounter += Time.deltaTime;
                frameCounter++;
            } else
            {
                lastFrameRate = frameCounter / timeCounter;
                frameCounter = 0;
                timeCounter = 0.0f;
            }
            frameRate = (int)lastFrameRate; // cast lastFrameRate as an int;
        }

        private IEnumerator WaitThenActive() // wait a certain amount of time before activating the pause mechanism
        {
            Plugin.Log.Debug("Waiting for the start of the level...");
            yield return new WaitForSeconds(waitThenActiveTime);
            activePause = true;
        }

        private IEnumerator WaitThenReActivate() // wait 2 seconds before reactivating the pause mechanism after its been fired
        {
            Plugin.Log.Debug("Resuming complete, reactivating active pause in 2 seconds");
            yield return new WaitForSeconds(2);
            activePause = true;
        }

        private void CheckEvents() // simply checks the events
        {
            BSEvents.gameSceneLoaded += OnLevelStart; 
            BSEvents.levelFailed += OnLevelFail;
            BSEvents.levelCleared += OnLevelClear;
            BSEvents.levelQuit += OnLevelQuit;
        }
        private void OnLevelStart() // level start delegate
        {
            //Plugin.Log.Debug("Level Started");
            isLevel = true;

            PauseController = Resources.FindObjectsOfTypeAll<PauseController>().FirstOrDefault();
            if (PauseController == null)
            {
                CriticalErrorHandler(true, 219);
            }

        }

        private void OnLevelFail(StandardLevelScenesTransitionSetupDataSO unused, LevelCompletionResults unusedResult) // level failed delegate
        {
            //Plugin.Log.Debug("LevelFailed");
            isLevel = false;
            activePause = false;
        }

        private void OnLevelClear(StandardLevelScenesTransitionSetupDataSO unused, LevelCompletionResults unusedResult) // level cleared delegate
        {
            //Plugin.Log.Debug("Level Ended");
            isLevel = false;
            activePause = false;
        }

        private void OnLevelQuit(StandardLevelScenesTransitionSetupDataSO unused, LevelCompletionResults unusedResult) // level quit delegate
        {
            //Plugin.Log.Debug("Level Quit");
            isLevel = false;
            activePause = false;
        }

        private void OnLevelResume() // level resumed delegate
        {
            StartCoroutine(WaitThenReActivate());
        }
        
        private  void CriticalErrorHandler(bool error, int lineNumber = 0, Exception exception = null) // i overdid this but idrc
        {
            int _case = 0;
            if (lineNumber == 0)
            {
                _case = 1;
            } 
            if(exception == null)
            {
                _case = 2;
            }
            if (exception != null && lineNumber != 0)
            {
                _case = 3;
            }
            if (error)
            {
                switch (_case)
                {
                    default:
                        Plugin.Log.Warn("A critical error has been encountered, disabling mod...");
                        break;
                    case 0:
                        Plugin.Log.Warn("A critical error has been encountered, disabling mod...");
                        break;
                    case 1:
                        Plugin.Log.Warn("A critical error has been encountered, disabling mod...");
                        Plugin.Log.Error(exception);
                        break;
                    case 2:
                        Plugin.Log.Warn("A critical error has been encountered around line " + lineNumber + ", disabling mod.");
                        break;
                    case 3:
                        Plugin.Log.Warn("A critical error has been encountered around line " + lineNumber + ", disabling mod.");
                        Plugin.Log.Error(exception);
                        break;

                }
                criticalError = true;
                modEnabled = false;
            }
        }
    }
}
