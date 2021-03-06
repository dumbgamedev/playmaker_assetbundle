// (c) Copyright HutongGames, LLC 2010-2017. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Asset Bundle")]
    [Tooltip("Checks if an AssetBundle contains an object by string name.")]
    public class CheckBundleContainsObject : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Asset name within the bundle to check using a string. This is case sensitive.")]
        public FsmString AssetName;

        [RequiredField]
        [Tooltip("Asset bundle to check.")]
        [UIHint(UIHint.Variable)]
        public FsmObject AssetBundle;

        [ActionSection("Output")]
        [Tooltip("Returns true if the asset is found within the bundle.")]
        [UIHint(UIHint.Variable)]
        public FsmBool containsObject;

        [ActionSection("Events")]
        [Tooltip("Event to fire on object found success.")]
        public FsmEvent objectFound;

        [Tooltip("Event to fire on object found failure.")]
        public FsmEvent objectNotFound;

        private AssetBundle _bundle;

        public override void Reset()
        {
            AssetBundle = null;
            containsObject = false;
            AssetName = null;
            objectFound = null;
            objectNotFound = null;
        }

        public override void OnEnter()
        {
            checkBundle();
        }


        void checkBundle()
        {
            _bundle = (AssetBundle) AssetBundle.Value;
            containsObject.Value = _bundle.Contains(AssetName.Value);
            if (_bundle)
            {
                Fsm.Event(objectFound);
            }
            else
            {
                Fsm.Event(objectNotFound);
            }
        }
    }
}