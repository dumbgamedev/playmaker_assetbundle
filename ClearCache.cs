// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Asset Bundle")]
    [Tooltip("Removes all AssetBundle content that has been cached by the current application.")]
    public class ClearCache : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Immediately removes all AssetBundle content that has been cached by the current application.")]
        public FsmBool clearImmediately;

        [RequiredField]
        [Tooltip("The number of seconds that AssetBundles may remain unused in the cache.")]
        public FsmInt remainInCacheTime;

        [ActionSection("Events")]
        [Tooltip("Event will fire if cache is cleared successfully.")]
        public FsmEvent clearSuccess;

        [Tooltip("Event will fire if cache failed to clear.")]
        public FsmEvent clearFailure;

        private bool success;

        private AssetBundle _bundle;

        public override void Reset()
        {
            remainInCacheTime = 0;
            clearImmediately = true;
            clearSuccess = null;
            clearFailure = null;
        }

        public override void OnEnter()
        {
            ClearAssetCache();
        }


        void ClearAssetCache()
        {
            if (clearImmediately.Value)
            {
                success = Caching.ClearCache();
            }
            else
            {
                success = Caching.ClearCache(remainInCacheTime.Value);
            }

            if (success)
            {
                Fsm.Event(clearSuccess);
            }
            else
            {
                Fsm.Event(clearFailure);
            }
        }
    }
}