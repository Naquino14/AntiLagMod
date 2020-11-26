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

        private bool criticalError;

        private bool modEnabled;
        private float frameThreshold;
        private bool frameDropDetection;
        private float waitThenActiveTime;

        private bool isLevel;
        private bool test = true;

        private DateTime lastTime;
        private int framesRendered;
        private int frameRate;

        private bool activePause = false;
        private bool waitThenActiveFireOnce;

        StandardLevelGameplayManager _gameplayManager;
        StandardLevelGameplayManager gameplayManager
        {
            get
            {
                if (_gameplayManager == null)
                {
                    _gameplayManager = Resources.FindObjectsOfTypeAll<StandardLevelGameplayManager>().FirstOrDefault();
                }
                return _gameplayManager;
            }
        }
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
        }
        private void Update()
        {
            CheckFrameRate();
            CheckEvents();
            if (isLevel && modEnabled)
            {

                Plugin.Log.Debug("FR: " + frameRate);
                if (waitThenActiveFireOnce)
                {
                    StartCoroutine(WaitThenActive());
                    waitThenActiveFireOnce = false;
                }

                if (activePause && (frameRate < frameThreshold))
                {
                    activePause = false;
                    PauseController.Pause();
                }

                if (!activePause)
                {
                    PauseController.didResumeEvent += OnLevelResume;
                }
                    
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

        public static void Refresh() // refresh the class variables to equal the property variables
        {
            // I farted really hard when I wrote this
            Instance.modEnabled = Configuration.ModEnabled;
            Instance.frameDropDetection = Configuration.FrameDropDetectionEnabled;
            Instance.frameThreshold = Configuration.FrameThreshold;
            Instance.waitThenActiveTime = Configuration.WaitThenActive;
        }

        private void CheckFrameRate()
        {
            framesRendered++;
            if((DateTime.Now - lastTime).TotalSeconds >= 1)
            {
                // one second elapsed
                frameRate = framesRendered;
                framesRendered = 0;
                lastTime = DateTime.Now;
            }
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
                Plugin.Log.Error("Could not find PauseController object... Disabling mod...");
                modEnabled = false;
                criticalError = true;
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
    }
}
