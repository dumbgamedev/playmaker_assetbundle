// (c) Copyright HutongGames, LLC 2010-2017. All rights reserved.

using System;
using UnityEngine;
using System.Collections;
using System.IO;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Asset Bundle")]
    [Tooltip(
        "Synchronously or Asynchronously load an Asset Bundle from a file on a local disk. This function supports bundles of any compression type. Asset Bundles should be saved to /StreamingAssets/ folder of your project.")]
    public class AssetBundleLoadFromFile : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Name of the asset bundle to load. This field is case sensitive. Typically this will be loaded from a folder in assets named 'StreamingAssets'")]
        [Title("Bundle Name")]
        public FsmString myassetBundle;

        [ActionSection("Events")]
        [Tooltip("Event to fire on load success.")]
        public FsmEvent loadSuccess;

        [Tooltip("Event to fire on load failure.")]
        public FsmEvent loadFailed;

        [ActionSection("Output")]
        [RequiredField]
        [Tooltip(
            "Saved asset bundle. Important: Create an playmaker object variable with the object type of UnityEngine/AssetBundle")]
        [Title("Asset Bundle")]
        [UIHint(UIHint.Variable)]
        public FsmObject myLoadedAssetBundle;

        [Tooltip(
            "Optionally save the progress of loading. Only avaliable in Async mode. Tip: Great for building loading bars.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat ASyncProgress;

        [ActionSection("Options")]
        [Tooltip("Set to true to use Asynchronously loading.")]
        public FsmBool useAsync;

        [Tooltip("Set to true for optional debug messages in the console. Turn off for builds.")]
        public FsmBool enableDebug;

        // private variables
        private AssetBundle _bundle;
        private AssetBundleCreateRequest bundleLoadRequest;

        public override void Reset()
        {
            useAsync = false;
            enableDebug = false;
            myLoadedAssetBundle = null;
            myassetBundle = new FsmString {UseVariable = false};
            loadFailed = null;
            loadSuccess = null;
            ASyncProgress = null;
        }

        public override void OnEnter()
        {
            // Use load sync
            if (!useAsync.Value)
            {
                syncLoad();
            }

            // use async loading
            else
            {
                StartCoroutine(LoadBundleAsync());
            }
        }


        private void syncLoad()
        {
            _bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, myassetBundle.Value));

            // fire is asset bundle fails to load
            if (_bundle == null)
            {
                if (enableDebug.Value)
                {
                    Debug.Log("Failed to load AssetBundle.");
                }

                Fsm.Event(loadFailed);
            }

            // bunlde load sucess
            else
            {
                myLoadedAssetBundle.Value = _bundle;

                if (enableDebug.Value)
                {
                    Debug.Log("AssetBundle load success.");
                }

                Fsm.Event(loadSuccess);
            }
        }

        private IEnumerator LoadBundleAsync()
        {
            bundleLoadRequest =
                AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, myassetBundle.Value));
            Debug.Log("Aysnc Started");

            yield return bundleLoadRequest;

            _bundle = bundleLoadRequest.assetBundle;
            Debug.Log(_bundle);

            // Asset bundle loaded successfully
            if (bundleLoadRequest.isDone && _bundle != null)
            {
                if (enableDebug.Value)
                {
                    Debug.Log("AssetBundle load success.");
                }

                myLoadedAssetBundle.Value = _bundle;
                Fsm.Event(loadSuccess);
            }

            // If bundle returned is null, fail load.
            if (bundleLoadRequest.isDone && _bundle == null)
            {
                if (enableDebug.Value)
                {
                    Debug.Log("Failed to load AssetBundle.");
                }

                Fsm.Event(loadFailed);
            }
        }

        public override void OnUpdate()
        {
            // For async, use loading bar.
            if (useAsync.Value)
            {
                ASyncProgress.Value = bundleLoadRequest.progress * 100f;

                if (enableDebug.Value)
                {
                    Debug.Log("Bundle Load Progress: " + bundleLoadRequest.progress * 100f);
                }
            }
        }
    }
}