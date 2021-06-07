﻿
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace NukoTween
{
    /// <summary>
    /// Tween機能を提供するUdonBehaviour
    /// </summary>
    public class NukoTweenEngine : UdonSharpBehaviour
    {
        //======================================================================================
        // 外部公開変数
        //======================================================================================
        #region public variables 
        /// <summary>tweenの同時実行数</summary>
        public int simultaneousSize = 128;
        #endregion


        //======================================================================================
        // 内部状態変数
        //======================================================================================
        #region private variables
        //
        // 命令を格納する為の配列(双方向リスト)
        //

        // tweenそれぞれで一意のID
        private int[] tweenIdCollection;

        // 現在動作中のtweenであるかどうか
        private bool[] workingCollection;

        // アクションの種類
        private int[] actionCollection;

        // 相対的な値で動作するかどうか
        private bool[] relativeCollection;

        // ループの種類
        private int[] loopModeCollection;

        // ループの残り回数
        private int[] loopCountCollection;

        // 操作対象 
        private GameObject[] targetCollection;
        private RectTransform[] targetRectTransformCollection;
        private Graphic[] targetGraphicCollection;
        private AudioSource[] targetAudioSourceCollection;
        private Text[] targetTextCollection;
        private TextMeshProUGUI[] targetTMPCollection;

        // tween前の状態
        private Vector3[] fromVector3Collection;
        private Color[] fromColorCollection;
        private Quaternion[] fromQuaternionCollection;

        // tween後の状態
        private Vector3[] toVector3Collection;
        private Quaternion[] toQuaternionCollection;
        private Color[] toColorCollection;
        private string[] toStringCollection;
        private bool[] toBoolCollection;

        // tweenの実行を開始した時間
        private float[] startTimeCollection;

        // tweenの実行にかかる時間
        private float[] durationCollection;

        // イージング関数のID
        private int[] easeIdCollection;

        // 次のノード
        private int[] nextIndexCollection;

        // 前のノード
        private int[] previousIndexCollection;

        /// <summary>先頭ノードのインデックス</summary>
        private int beginCollectionIndex = -1;

        /// <summary>末尾ノードのインデックス</summary>
        private int endCollectionIndex = -1;

        /// <summary>現在登録中のtweenの数</summary>
        private int numberOfTweening = 0;

        /// <summary>一番最後に登録されたtweenのID</summary>
        private int currentTweenId = 0;
        #endregion


        //======================================================================================
        // イージング関数を指定する為のID
        //======================================================================================
        #region enum Ease
        public readonly int EaseLinear       =   0;
        public readonly int EaseInSine       =  10;
        public readonly int EaseOutSine      =  11;
        public readonly int EaseInOutSine    =  12;
        public readonly int EaseInQuad       =  20;
        public readonly int EaseOutQuad      =  21;
        public readonly int EaseInOutQuad    =  22;
        public readonly int EaseInCubic      =  30;
        public readonly int EaseOutCubic     =  31;
        public readonly int EaseInOutCubic   =  32;
        public readonly int EaseInQuart      =  40;
        public readonly int EaseOutQuart     =  41;
        public readonly int EaseInOutQuart   =  42;
        public readonly int EaseInQuint      =  50;
        public readonly int EaseOutQuint     =  51;
        public readonly int EaseInOutQuint   =  52;
        public readonly int EaseInExpo       =  60;
        public readonly int EaseOutExpo      =  61;
        public readonly int EaseInOutExpo    =  62;
        public readonly int EaseInCirc       =  70;
        public readonly int EaseOutCirc      =  71;
        public readonly int EaseInOutCirc    =  72;
        public readonly int EaseInBack       =  80;
        public readonly int EaseOutBack      =  81;
        public readonly int EaseInOutBack    =  82;
        public readonly int EaseInElastic    =  90;
        public readonly int EaseOutElastic   =  91;
        public readonly int EaseInOutElastic =  92;
        public readonly int EaseInBounce     = 100;
        public readonly int EaseOutBounce    = 101;
        public readonly int EaseInOutBounce  = 102;
        #endregion


        //======================================================================================
        // アクションを指定する為のID
        //======================================================================================
        #region enum Action
        private const int ActionNone             =   0;
        private const int ActionLocalMove        = 100;
        private const int ActionMove             = 101;
        private const int ActionLocalRotate      = 200;
        private const int ActionRotate           = 202;
        private const int ActionLocalScale       = 300;
        private const int ActionAnchorPos        = 400;
        private const int ActionColor            = 401;
        private const int ActionFadeGraphic      = 402;
        private const int ActionFillAmount       = 403;
        private const int ActionText             = 404;
        private const int ActionTextTMP          = 405;
        private const int ActionFadeVolume       = 406;
        private const int ActionDelayedSetActive = 900;
        #endregion


        //======================================================================================
        // ループを指定する為のID
        //======================================================================================
        #region enum LoopMode
        private const int LoopModeNone        = 0;
        private const int LoopModeRestart     = 1;
        private const int LoopModeReverse     = 2;
        private const int LoopModeIncremental = 3;
        #endregion


        //======================================================================================
        // Unity(Udon)のライフサイクルイベント
        //======================================================================================
        #region lifecycle events
        private void Start()
        {
            tweenIdCollection = new int[simultaneousSize];
            workingCollection = new bool[simultaneousSize];
            actionCollection = new int[simultaneousSize];
            relativeCollection = new bool[simultaneousSize];
            loopModeCollection = new int[simultaneousSize];
            loopCountCollection = new int[simultaneousSize];
            targetCollection = new GameObject[simultaneousSize];
            targetRectTransformCollection = new RectTransform[simultaneousSize];
            targetGraphicCollection = new Graphic[simultaneousSize];
            targetAudioSourceCollection = new AudioSource[simultaneousSize];
            targetTextCollection = new Text[simultaneousSize];
            targetTMPCollection = new TextMeshProUGUI[simultaneousSize];
            fromVector3Collection = new Vector3[simultaneousSize];
            fromQuaternionCollection = new Quaternion[simultaneousSize];
            fromColorCollection = new Color[simultaneousSize];
            toVector3Collection = new Vector3[simultaneousSize];
            toQuaternionCollection = new Quaternion[simultaneousSize];
            toColorCollection = new Color[simultaneousSize];
            toStringCollection = new string[simultaneousSize];
            toBoolCollection = new bool[simultaneousSize];
            startTimeCollection = new float[simultaneousSize];
            durationCollection = new float[simultaneousSize];
            easeIdCollection = new int[simultaneousSize];
            nextIndexCollection = new int[simultaneousSize];
            previousIndexCollection = new int[simultaneousSize];

            for (var i = 0; i < simultaneousSize; i++)
            {
                nextIndexCollection[i] = -1;
                previousIndexCollection[i] = -1;
            }
        }

        private void Update()
        {
            if (numberOfTweening == 0) return;

            var index = beginCollectionIndex;
            var nextIndex = -1;

            for (var i = 0; i < simultaneousSize; i++)
            {
                nextIndex = nextIndexCollection[index];

                ExecuteAction(index, false);

                index = nextIndex;
                if (index == -1)
                {
                    break;
                }
            }
        }
        #endregion


        //======================================================================================
        // アクションを登録する為のpublicメソッドと、アクションを実行するprivateメソッド
        //======================================================================================
        #region action methods
        private void ExecuteAction(int index, bool isRequestComplete)
        {
            switch (actionCollection[index])
            {
                case ActionNone:
                    break;

                case ActionLocalMove:
                    ExecuteActionLocalMove(index, isRequestComplete);
                    break;

                case ActionMove:
                    ExecuteActionMove(index, isRequestComplete);
                    break;

                case ActionAnchorPos:
                    ExecuteActionAnchorPos(index, isRequestComplete);
                    break;

                case ActionLocalRotate:
                    ExecuteActionLocalRotate(index, isRequestComplete);
                    break;

                case ActionRotate:
                    ExecuteActionRotate(index, isRequestComplete);
                    break;

                case ActionLocalScale:
                    ExecuteActionLocalScale(index, isRequestComplete);
                    break;

                case ActionColor:
                    ExecuteActionColor(index, isRequestComplete);
                    break;

                case ActionFadeGraphic:
                    ExecuteActionFadeGraphic(index, isRequestComplete);
                    break;

                case ActionFillAmount:
                    ExecuteActionFillAmount(index, isRequestComplete);
                    break;

                case ActionText:
                    ExecuteActionText(index, isRequestComplete);
                    break;

                case ActionTextTMP:
                    ExecuteActionTextTMP(index, isRequestComplete);
                    break;

                case ActionFadeVolume:
                    ExecuteActionFadeVolume(index, isRequestComplete);
                    break;

                case ActionDelayedSetActive:
                    ExecuteDelayedSetActive(index, isRequestComplete);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// GameObjectのLocalPositionをTweenする
        /// </summary>
        /// <param name="target">tween対象</param>
        /// <param name="to">移動先</param>
        /// <param name="duration">tweenにかける時間(s)</param>
        /// <param name="delay">tween開始を遅らせる時間(s)</param>
        /// <param name="easeId">イージング関数(tween.EaseXXX)</param>
        /// <param name="relative">相対的な動作を行うかどうか</param>
        /// <returns>tweenId</returns>
        public int LocalMoveTo(GameObject target, Vector3 to, float duration, float delay, int easeId, bool relative)
        {
            if (!ValidateRegisterAction()) return -1;

            RegisterAction();

            actionCollection[endCollectionIndex] = ActionLocalMove;
            relativeCollection[endCollectionIndex] = relative;
            targetCollection[endCollectionIndex] = target;
            toVector3Collection[endCollectionIndex] = to;
            durationCollection[endCollectionIndex] = duration;
            startTimeCollection[endCollectionIndex] += delay;
            easeIdCollection[endCollectionIndex] = easeId;

            return currentTweenId;
        }

        private void ExecuteActionLocalMove(int index, bool isRequestComplete)
        {
            var startTime = startTimeCollection[index];

            if (Time.time < startTime && !isRequestComplete)
            {
                return;
            }

            var target = targetCollection[index];

            if (!workingCollection[index])
            {
                workingCollection[index] = true;
                fromVector3Collection[index] = target.transform.localPosition;

                if (relativeCollection[index])
                {
                    toVector3Collection[index] = target.transform.localPosition + toVector3Collection[index];
                }
            }

            var delta = Time.time - startTime;
            var dulation = durationCollection[index];

            if (delta < dulation && !isRequestComplete)
            {
                var ratio = delta / dulation;
                var easeRatio = Ease(easeIdCollection[index], ratio);
                target.transform.localPosition = Vector3.Lerp(fromVector3Collection[index], toVector3Collection[index], easeRatio);
            }
            else
            {
                target.transform.localPosition = toVector3Collection[index];

                if (loopCountCollection[index] == 0)
                {
                    UnregisterAction(index);
                }
                else
                {
                    startTimeCollection[index] = startTime + dulation;

                    var loopMode = loopModeCollection[index];

                    if (loopMode == LoopModeRestart)
                    {
                        // Empty
                    }
                    else if (loopMode == LoopModeReverse)
                    {
                        var tmp = fromVector3Collection[index];
                        fromVector3Collection[index] = toVector3Collection[index];
                        toVector3Collection[index] = tmp;
                    }
                    else if (loopMode == LoopModeIncremental)
                    {
                        var tmp = toVector3Collection[index] - fromVector3Collection[index];
                        fromVector3Collection[index] = toVector3Collection[index];
                        toVector3Collection[index] = toVector3Collection[index] + tmp;
                    }
                    else
                    {
                        UnregisterAction(index);
                    }

                    loopCountCollection[index] = Mathf.Max(-1, loopCountCollection[index] - 1);
                }
            }
        }

        /// <summary>
        /// GameObjectのPositionをTweenする
        /// </summary>
        /// <param name="target">tween対象</param>
        /// <param name="to">移動先</param>
        /// <param name="duration">tweenにかける時間(s)</param>
        /// <param name="delay">tween開始を遅らせる時間(s)</param>
        /// <param name="easeId">イージング関数(tween.EaseXXX)</param>
        /// <param name="relative">相対的な動作を行うかどうか</param>
        /// <returns>tweenId</returns>
        public int MoveTo(GameObject target, Vector3 to, float duration, float delay, int easeId, bool relative)
        {
            if (!ValidateRegisterAction()) return -1;

            RegisterAction();

            actionCollection[endCollectionIndex] = ActionMove;
            relativeCollection[endCollectionIndex] = relative;
            targetCollection[endCollectionIndex] = target;
            toVector3Collection[endCollectionIndex] = to;
            durationCollection[endCollectionIndex] = duration;
            startTimeCollection[endCollectionIndex] += delay;
            easeIdCollection[endCollectionIndex] = easeId;

            return currentTweenId;
        }

        private void ExecuteActionMove(int index, bool isRequestComplete)
        {
            var startTime = startTimeCollection[index];

            if (Time.time < startTime && !isRequestComplete)
            {
                return;
            }

            var target = targetCollection[index];

            if (!workingCollection[index])
            {
                workingCollection[index] = true;
                fromVector3Collection[index] = target.transform.position;

                if (relativeCollection[index])
                {
                    toVector3Collection[index] = target.transform.position + toVector3Collection[index];
                }
            }

            var delta = Time.time - startTime;
            var dulation = durationCollection[index];

            if (delta < dulation && !isRequestComplete)
            {
                var ratio = delta / dulation;
                var easeRatio = Ease(easeIdCollection[index], ratio);
                target.transform.position = Vector3.Lerp(fromVector3Collection[index], toVector3Collection[index], easeRatio);
            }
            else
            {
                target.transform.position = toVector3Collection[index];

                if (loopCountCollection[index] == 0)
                {
                    UnregisterAction(index);
                }
                else
                {
                    startTimeCollection[index] = startTime + dulation;

                    var loopMode = loopModeCollection[index];

                    if (loopMode == LoopModeRestart)
                    {
                        // Empty
                    }
                    else if (loopMode == LoopModeReverse)
                    {
                        var tmp = fromVector3Collection[index];
                        fromVector3Collection[index] = toVector3Collection[index];
                        toVector3Collection[index] = tmp;
                    }
                    else if (loopMode == LoopModeIncremental)
                    {
                        var tmp = toVector3Collection[index] - fromVector3Collection[index];
                        fromVector3Collection[index] = toVector3Collection[index];
                        toVector3Collection[index] = toVector3Collection[index] + tmp;
                    }
                    else
                    {
                        UnregisterAction(index);
                    }

                    loopCountCollection[index] = Mathf.Max(-1, loopCountCollection[index] - 1);
                }
            }
        }

        /// <summary>
        /// GameObjectのAnchoredPositionをTweenする
        /// </summary>
        /// <param name="target">tween対象</param>
        /// <param name="to">移動先</param>
        /// <param name="duration">tweenにかける時間(s)</param>
        /// <param name="delay">tween開始を遅らせる時間(s)</param>
        /// <param name="easeId">イージング関数(tween.EaseXXX)</param>
        /// <param name="relative">相対的な動作を行うかどうか</param>
        /// <returns>tweenId</returns>
        public int AnchorPosTo(GameObject target, Vector3 to, float duration, float delay, int easeId, bool relative)
        {
            if (!ValidateRegisterAction()) return -1;

            var rectTransform = target.GetComponent<RectTransform>();

            if (rectTransform == null)
            {
                LogError("引数" + nameof(target) + "のGameObjectにRectTransformコンポーネントがありません");
            }

            RegisterAction();

            actionCollection[endCollectionIndex] = ActionAnchorPos;
            relativeCollection[endCollectionIndex] = relative;
            targetRectTransformCollection[endCollectionIndex] = rectTransform;
            toVector3Collection[endCollectionIndex] = to;
            durationCollection[endCollectionIndex] = duration;
            startTimeCollection[endCollectionIndex] += delay;
            easeIdCollection[endCollectionIndex] = easeId;

            return currentTweenId;
        }

        private void ExecuteActionAnchorPos(int index, bool isRequestComplete)
        {
            var startTime = startTimeCollection[index];

            if (Time.time < startTime && !isRequestComplete)
            {
                return;
            }

            var rectTransform = targetRectTransformCollection[index];

            if (!workingCollection[index])
            {
                workingCollection[index] = true;
                fromVector3Collection[index] = rectTransform.anchoredPosition3D;

                if (relativeCollection[index])
                {
                    toVector3Collection[index] = rectTransform.anchoredPosition3D + toVector3Collection[index];
                }
            }

            var delta = Time.time - startTime;
            var dulation = durationCollection[index];

            if (delta < dulation && !isRequestComplete)
            {
                var ratio = delta / dulation;
                var easeRatio = Ease(easeIdCollection[index], ratio);
                rectTransform.anchoredPosition3D = Vector3.Lerp(fromVector3Collection[index], toVector3Collection[index], easeRatio);
            }
            else
            {
                rectTransform.anchoredPosition3D = toVector3Collection[index];

                if (loopCountCollection[index] == 0)
                {
                    UnregisterAction(index);
                }
                else
                {
                    startTimeCollection[index] = startTime + dulation;

                    var loopMode = loopModeCollection[index];

                    if (loopMode == LoopModeRestart)
                    {
                        // Empty
                    }
                    else if (loopMode == LoopModeReverse)
                    {
                        var tmp = fromVector3Collection[index];
                        fromVector3Collection[index] = toVector3Collection[index];
                        toVector3Collection[index] = tmp;
                    }
                    else if (loopMode == LoopModeIncremental)
                    {
                        var tmp = toVector3Collection[index] - fromVector3Collection[index];
                        fromVector3Collection[index] = toVector3Collection[index];
                        toVector3Collection[index] = toVector3Collection[index] + tmp;
                    }
                    else
                    {
                        UnregisterAction(index);
                    }

                    loopCountCollection[index] = Mathf.Max(-1, loopCountCollection[index] - 1);
                }
            }
        }

        /// <summary>
        /// GameObjectのLocalScaleをTweenする
        /// </summary>
        /// <param name="target">tween対象</param>
        /// <param name="to">スケール変更後</param>
        /// <param name="duration">tweenにかける時間(s)</param>
        /// <param name="delay">tween開始を遅らせる時間(s)</param>
        /// <param name="easeId">イージング関数(tween.EaseXXX)</param>
        /// <param name="relative">相対的な動作を行うかどうか</param>
        /// <returns>tweenId</returns>
        public int LocalScaleTo(GameObject target, Vector3 to, float duration, float delay, int easeId, bool relative)
        {
            if (!ValidateRegisterAction()) return -1;

            RegisterAction();

            actionCollection[endCollectionIndex] = ActionLocalScale;
            relativeCollection[endCollectionIndex] = relative;
            targetCollection[endCollectionIndex] = target;
            toVector3Collection[endCollectionIndex] = to;
            durationCollection[endCollectionIndex] = duration;
            startTimeCollection[endCollectionIndex] += delay;
            easeIdCollection[endCollectionIndex] = easeId;

            return currentTweenId;
        }

        private void ExecuteActionLocalScale(int index, bool isRequestComplete)
        {
            var startTime = startTimeCollection[index];

            if (Time.time < startTime && !isRequestComplete)
            {
                return;
            }

            var target = targetCollection[index];

            if (!workingCollection[index])
            {
                workingCollection[index] = true;
                fromVector3Collection[index] = target.transform.localScale;

                if (relativeCollection[index])
                {
                    toVector3Collection[index] = target.transform.localScale + toVector3Collection[index];
                }
            }

            var delta = Time.time - startTime;
            var dulation = durationCollection[index];

            if (delta < dulation && !isRequestComplete)
            {
                var ratio = delta / dulation;
                var easeRatio = Ease(easeIdCollection[index], ratio);
                target.transform.localScale = Vector3.Lerp(fromVector3Collection[index], toVector3Collection[index], easeRatio);
            }
            else
            {
                target.transform.localScale = toVector3Collection[index];

                if (loopCountCollection[index] == 0)
                {
                    UnregisterAction(index);
                }
                else
                {
                    startTimeCollection[index] = startTime + dulation;

                    var loopMode = loopModeCollection[index];

                    if (loopMode == LoopModeRestart)
                    {
                        // Empty
                    }
                    else if (loopMode == LoopModeReverse)
                    {
                        var tmp = fromVector3Collection[index];
                        fromVector3Collection[index] = toVector3Collection[index];
                        toVector3Collection[index] = tmp;
                    }
                    else if (loopMode == LoopModeIncremental)
                    {
                        var tmp = toVector3Collection[index] - fromVector3Collection[index];
                        fromVector3Collection[index] = toVector3Collection[index];
                        toVector3Collection[index] = toVector3Collection[index] + tmp;
                    }
                    else
                    {
                        UnregisterAction(index);
                    }

                    loopCountCollection[index] = Mathf.Max(-1, loopCountCollection[index] - 1);
                }
            }
        }

        /// <summary>
        /// GameObjectのLocalRotateをTweenする
        /// </summary>
        /// <param name="target">tween対象</param>
        /// <param name="to">回転後のオイラー角</param>
        /// <param name="duration">tweenにかける時間(s)</param>
        /// <param name="delay">tween開始を遅らせる時間(s)</param>
        /// <param name="easeId">イージング関数(tween.EaseXXX)</param>
        /// <param name="relative">相対的な動作を行うかどうか</param>
        /// <returns>tweenId</returns>
        public int LocalRotateTo(GameObject target, Vector3 to, float duration, float delay, int easeId, bool relative)
        {
            return LocalRotateQuaternionTo(target, Quaternion.Euler(to), duration, delay, easeId, relative);
        }

        /// <summary>
        /// GameObjectのLocalRotateをTweenする
        /// </summary>
        /// <param name="target">tween対象</param>
        /// <param name="to">回転後のクォータニオン</param>
        /// <param name="duration">tweenにかける時間(s)</param>
        /// <param name="delay">tween開始を遅らせる時間(s)</param>
        /// <param name="easeId">イージング関数(tween.EaseXXX)</param>
        /// <param name="relative">相対的な動作を行うかどうか</param>
        /// <returns>tweenId</returns>
        public int LocalRotateQuaternionTo(GameObject target, Quaternion to, float duration, float delay, int easeId, bool relative)
        {
            if (!ValidateRegisterAction()) return -1;

            RegisterAction();

            actionCollection[endCollectionIndex] = ActionLocalRotate;
            relativeCollection[endCollectionIndex] = relative;
            targetCollection[endCollectionIndex] = target;
            toQuaternionCollection[endCollectionIndex] = to;
            durationCollection[endCollectionIndex] = duration;
            startTimeCollection[endCollectionIndex] += delay;
            easeIdCollection[endCollectionIndex] = easeId;

            return currentTweenId;
        }

        private void ExecuteActionLocalRotate(int index, bool isRequestComplete)
        {
            var startTime = startTimeCollection[index];

            if (Time.time < startTime && !isRequestComplete)
            {
                return;
            }

            var target = targetCollection[index];

            if (!workingCollection[index])
            {
                workingCollection[index] = true;
                fromQuaternionCollection[index] = target.transform.localRotation;

                if (relativeCollection[index])
                {
                    toQuaternionCollection[index] = target.transform.localRotation * toQuaternionCollection[index];
                }
            }

            var delta = Time.time - startTime;
            var dulation = durationCollection[index];

            if (delta < dulation && !isRequestComplete)
            {
                var ratio = delta / dulation;
                var easeRatio = Ease(easeIdCollection[index], ratio);
                target.transform.localRotation = Quaternion.Lerp(fromQuaternionCollection[index], toQuaternionCollection[index], easeRatio);
            }
            else
            {
                target.transform.localRotation = toQuaternionCollection[index];

                if (loopCountCollection[index] == 0)
                {
                    UnregisterAction(index);
                }
                else
                {
                    startTimeCollection[index] = startTime + dulation;

                    var loopMode = loopModeCollection[index];

                    if (loopMode == LoopModeRestart)
                    {
                        // Empty
                    }
                    else if (loopMode == LoopModeReverse)
                    {
                        var tmp = fromQuaternionCollection[index];
                        fromQuaternionCollection[index] = toQuaternionCollection[index];
                        toQuaternionCollection[index] = tmp;
                    }
                    else if (loopMode == LoopModeIncremental)
                    {
                        var tmp = toQuaternionCollection[index] * Quaternion.Inverse(fromQuaternionCollection[index]);
                        fromQuaternionCollection[index] = toQuaternionCollection[index];
                        toQuaternionCollection[index] = toQuaternionCollection[index] * tmp;
                    }
                    else
                    {
                        UnregisterAction(index);
                    }

                    loopCountCollection[index] = Mathf.Max(-1, loopCountCollection[index] - 1);
                }
            }
        }

        /// <summary>
        /// GameObjectのRotateをTweenする
        /// </summary>
        /// <param name="target">tween対象</param>
        /// <param name="to">回転後のオイラー角</param>
        /// <param name="duration">tweenにかける時間(s)</param>
        /// <param name="delay">tween開始を遅らせる時間(s)</param>
        /// <param name="easeId">イージング関数(tween.EaseXXX)</param>
        /// <param name="relative">相対的な動作を行うかどうか</param>
        /// <returns>tweenId</returns>
        public int RotateTo(GameObject target, Vector3 to, float duration, float delay, int easeId, bool relative)
        {
            return RotateQuaternionTo(target, Quaternion.Euler(to), duration, delay, easeId, relative);
        }

        /// <summary>
        /// GameObjectのRotateをTweenする
        /// </summary>
        /// <param name="target">tween対象</param>
        /// <param name="to">回転後のクォータニオン</param>
        /// <param name="duration">tweenにかける時間(s)</param>
        /// <param name="delay">tween開始を遅らせる時間(s)</param>
        /// <param name="easeId">イージング関数(tween.EaseXXX)</param>
        /// <param name="relative">相対的な動作を行うかどうか</param>
        /// <returns>tweenId</returns>
        public int RotateQuaternionTo(GameObject target, Quaternion to, float duration, float delay, int easeId, bool relative)
        {
            if (!ValidateRegisterAction()) return -1;

            RegisterAction();

            actionCollection[endCollectionIndex] = ActionRotate;
            relativeCollection[endCollectionIndex] = relative;
            targetCollection[endCollectionIndex] = target;
            toQuaternionCollection[endCollectionIndex] = to;
            durationCollection[endCollectionIndex] = duration;
            startTimeCollection[endCollectionIndex] += delay;
            easeIdCollection[endCollectionIndex] = easeId;

            return currentTweenId;
        }

        private void ExecuteActionRotate(int index, bool isRequestComplete)
        {
            var startTime = startTimeCollection[index];

            if (Time.time < startTime && !isRequestComplete)
            {
                return;
            }

            var target = targetCollection[index];

            if (!workingCollection[index])
            {
                workingCollection[index] = true;
                fromQuaternionCollection[index] = target.transform.rotation;

                if (relativeCollection[index])
                {
                    toQuaternionCollection[index] = target.transform.rotation * toQuaternionCollection[index];
                }
            }

            var delta = Time.time - startTime;
            var dulation = durationCollection[index];

            if (delta < dulation && !isRequestComplete)
            {
                var ratio = delta / dulation;
                var easeRatio = Ease(easeIdCollection[index], ratio);
                target.transform.rotation = Quaternion.Lerp(fromQuaternionCollection[index], toQuaternionCollection[index], easeRatio);
            }
            else
            {
                target.transform.rotation = toQuaternionCollection[index];

                if (loopCountCollection[index] == 0)
                {
                    UnregisterAction(index);
                }
                else
                {
                    startTimeCollection[index] = startTime + dulation;

                    var loopMode = loopModeCollection[index];

                    if (loopMode == LoopModeRestart)
                    {
                        // Empty
                    }
                    else if (loopMode == LoopModeReverse)
                    {
                        var tmp = fromQuaternionCollection[index];
                        fromQuaternionCollection[index] = toQuaternionCollection[index];
                        toQuaternionCollection[index] = tmp;
                    }
                    else if (loopMode == LoopModeIncremental)
                    {
                        var tmp = toQuaternionCollection[index] * Quaternion.Inverse(fromQuaternionCollection[index]);
                        fromQuaternionCollection[index] = toQuaternionCollection[index];
                        toQuaternionCollection[index] = toQuaternionCollection[index] * tmp;
                    }
                    else
                    {
                        UnregisterAction(index);
                    }

                    loopCountCollection[index] = Mathf.Max(-1, loopCountCollection[index] - 1);
                }
            }
        }

        /// <summary>
        /// GraphicコンポーネントのColorをTweenする
        /// </summary>
        /// <param name="target">tween対象</param>
        /// <param name="to">変更後の色</param>
        /// <param name="duration">tweenにかける時間(s)</param>
        /// <param name="delay">tween開始を遅らせる時間(s)</param>
        /// <param name="easeId">イージング関数(tween.EaseXXX)</param>
        /// <returns>tweenId</returns>
        public int ColorTo(Graphic target, Color to, float duration, float delay, int easeId)
        {
            if (!ValidateRegisterAction()) return -1;

            RegisterAction();

            actionCollection[endCollectionIndex] = ActionColor;
            targetGraphicCollection[endCollectionIndex] = target;
            toColorCollection[endCollectionIndex] = to;
            durationCollection[endCollectionIndex] = duration;
            startTimeCollection[endCollectionIndex] += delay;
            easeIdCollection[endCollectionIndex] = easeId;

            return currentTweenId;
        }

        private void ExecuteActionColor(int index, bool isRequestComplete)
        {
            var startTime = startTimeCollection[index];

            if (Time.time < startTime && !isRequestComplete)
            {
                return;
            }

            var target = targetGraphicCollection[index];

            if (!workingCollection[index])
            {
                workingCollection[index] = true;
                fromColorCollection[index] = target.color;
            }

            var delta = Time.time - startTime;
            var dulation = durationCollection[index];

            if (delta < dulation && !isRequestComplete)
            {
                var ratio = delta / dulation;
                var easeRatio = Ease(easeIdCollection[index], ratio);
                target.color = Color.Lerp(fromColorCollection[index], toColorCollection[index], easeRatio);
            }
            else
            {
                target.color = toColorCollection[index];

                if (loopCountCollection[index] == 0)
                {
                    UnregisterAction(index);
                }
                else
                {
                    startTimeCollection[index] = startTime + dulation;

                    var loopMode = loopModeCollection[index];

                    if (loopMode == LoopModeRestart)
                    {
                        // Empty
                    }
                    else if (loopMode == LoopModeReverse)
                    {
                        var tmp = fromColorCollection[index];
                        fromColorCollection[index] = toColorCollection[index];
                        toColorCollection[index] = tmp;
                    }
                    else
                    {
                        UnregisterAction(index);
                    }

                    loopCountCollection[index] = Mathf.Max(-1, loopCountCollection[index] - 1);
                }
            }
        }

        /// <summary>
        /// GraphicコンポーネントのColorのAlpha値をTweenする
        /// </summary>
        /// <param name="target">tween対象</param>
        /// <param name="to">変更後のAlpha値</param>
        /// <param name="duration">tweenにかける時間(s)</param>
        /// <param name="delay">tween開始を遅らせる時間(s)</param>
        /// <param name="easeId">イージング関数(tween.EaseXXX)</param>
        /// <returns>tweenId</returns>
        public int FadeGraphicTo(Graphic target, float to, float duration, float delay, int easeId)
        {
            if (!ValidateRegisterAction()) return -1;

            RegisterAction();

            actionCollection[endCollectionIndex] = ActionFadeGraphic;
            targetGraphicCollection[endCollectionIndex] = target;
            toColorCollection[endCollectionIndex] = new Color(0f, 0f, 0f, to);
            durationCollection[endCollectionIndex] = duration;
            startTimeCollection[endCollectionIndex] += delay;
            easeIdCollection[endCollectionIndex] = easeId;

            return currentTweenId;
        }

        private void ExecuteActionFadeGraphic(int index, bool isRequestComplete)
        {
            var startTime = startTimeCollection[index];

            if (Time.time < startTime && !isRequestComplete)
            {
                return;
            }

            var target = targetGraphicCollection[index];

            if (!workingCollection[index])
            {
                workingCollection[index] = true;
                fromColorCollection[index] = target.color;
            }

            var delta = Time.time - startTime;
            var dulation = durationCollection[index];

            if (delta < dulation && !isRequestComplete)
            {
                var ratio = delta / dulation;
                var easeRatio = Ease(easeIdCollection[index], ratio);
                var color = targetGraphicCollection[index].color;
                color.a = Mathf.Lerp(fromColorCollection[index].a, toColorCollection[index].a, easeRatio);
                target.color = color;
            }
            else
            {
                var color = targetGraphicCollection[index].color;
                color.a = toColorCollection[index].a;
                target.color = color;

                if (loopCountCollection[index] == 0)
                {
                    UnregisterAction(index);
                }
                else
                {
                    startTimeCollection[index] = startTime + dulation;

                    var loopMode = loopModeCollection[index];

                    if (loopMode == LoopModeRestart)
                    {
                        // Empty
                    }
                    else if (loopMode == LoopModeReverse)
                    {
                        var tmp = fromColorCollection[index];
                        fromColorCollection[index] = toColorCollection[index];
                        toColorCollection[index] = tmp;
                    }
                    else
                    {
                        UnregisterAction(index);
                    }

                    loopCountCollection[index] = Mathf.Max(-1, loopCountCollection[index] - 1);
                }
            }
        }

        /// <summary>
        /// ImageコンポーネントのFillAmountをTweenする
        /// </summary>
        /// <param name="target">tween対象</param>
        /// <param name="to">変更後の値</param>
        /// <param name="duration">tweenにかける時間(s)</param>
        /// <param name="delay">tween開始を遅らせる時間(s)</param>
        /// <param name="easeId">イージング関数(tween.EaseXXX)</param>
        /// <returns>tweenId</returns>
        public int FillAmountTo(Image target, float to, float duration, float delay, int easeId)
        {
            if (!ValidateRegisterAction()) return -1;

            RegisterAction();

            actionCollection[endCollectionIndex] = ActionFillAmount;
            targetGraphicCollection[endCollectionIndex] = target;
            toVector3Collection[endCollectionIndex] = new Vector3(to, 0f);
            durationCollection[endCollectionIndex] = duration;
            startTimeCollection[endCollectionIndex] += delay;
            easeIdCollection[endCollectionIndex] = easeId;

            return currentTweenId;
        }

        private void ExecuteActionFillAmount(int index, bool isRequestComplete)
        {
            var startTime = startTimeCollection[index];

            if (Time.time < startTime && !isRequestComplete)
            {
                return;
            }

            var target = (Image)targetGraphicCollection[index];

            if (!workingCollection[index])
            {
                workingCollection[index] = true;
                fromVector3Collection[index] = new Vector3(target.fillAmount, 0f);
            }

            var delta = Time.time - startTime;
            var dulation = durationCollection[index];

            if (delta < dulation && !isRequestComplete)
            {
                var ratio = delta / dulation;
                var easeRatio = Ease(easeIdCollection[index], ratio);
                target.fillAmount = Mathf.Lerp(fromVector3Collection[index].x, toVector3Collection[index].x, easeRatio);
            }
            else
            {
                target.fillAmount = toVector3Collection[index].x;

                if (loopCountCollection[index] == 0)
                {
                    UnregisterAction(index);
                }
                else
                {
                    startTimeCollection[index] = startTime + dulation;

                    var loopMode = loopModeCollection[index];

                    if (loopMode == LoopModeRestart)
                    {
                        // Empty
                    }
                    else if (loopMode == LoopModeReverse)
                    {
                        var tmp = fromVector3Collection[index];
                        fromVector3Collection[index] = toVector3Collection[index];
                        toVector3Collection[index] = tmp;
                    }
                    else
                    {
                        UnregisterAction(index);
                    }

                    loopCountCollection[index] = Mathf.Max(-1, loopCountCollection[index] - 1);
                }
            }
        }

        /// <summary>
        /// TextコンポーネントのTextをTweenする
        /// </summary>
        /// <param name="target">tween対象</param>
        /// <param name="to">変更後の文字列</param>
        /// <param name="duration">tweenにかける時間(s)</param>
        /// <param name="delay">tween開始を遅らせる時間(s)</param>
        /// <param name="easeId">イージング関数(tween.EaseXXX)</param>
        /// <returns>tweenId</returns>
        public int TextTo(Text target, string to, float duration, float delay, int easeId)
        {
            if (!ValidateRegisterAction()) return -1;

            if (to.Length == 0)
            {
                LogError("引数" + nameof(to) + "にLengthが0の文字列は使用できません");
                return -1;
            }

            RegisterAction();

            actionCollection[endCollectionIndex] = ActionText;
            targetTextCollection[endCollectionIndex] = target;
            toStringCollection[endCollectionIndex] = to;
            durationCollection[endCollectionIndex] = duration;
            startTimeCollection[endCollectionIndex] += delay;
            easeIdCollection[endCollectionIndex] = easeId;

            return currentTweenId;
        }

        private void ExecuteActionText(int index, bool isRequestComplete)
        {
            var startTime = startTimeCollection[index];

            if (Time.time < startTime && !isRequestComplete)
            {
                return;
            }

            var target = targetTextCollection[index];

            workingCollection[index] = true;

            var delta = Time.time - startTime;
            var dulation = durationCollection[index];

            if (delta < dulation && !isRequestComplete)
            {
                var ratio = delta / dulation;
                var easeRatio = Ease(easeIdCollection[index], ratio);
                target.text = toStringCollection[index].Substring(0, (int)(toStringCollection[index].Length * easeRatio));
            }
            else
            {
                target.text = toStringCollection[index];

                UnregisterAction(index);
            }
        }

        /// <summary>
        /// TextMeshProコンポーネントのTextをTweenする
        /// </summary>
        /// <param name="target">tween対象</param>
        /// <param name="to">変更後の文字列</param>
        /// <param name="duration">tweenにかける時間(s)</param>
        /// <param name="delay">tween開始を遅らせる時間(s)</param>
        /// <param name="easeId">イージング関数(tween.EaseXXX)</param>
        /// <returns>tweenId</returns>
        public int TextTMPTo(TextMeshProUGUI target, string to, float duration, float delay, int easeId)
        {
            if (!ValidateRegisterAction()) return -1;

            if (to.Length == 0)
            {
                LogError("引数" + nameof(to) + "にLengthが0の文字列は使用できません");
                return -1;
            }

            RegisterAction();

            actionCollection[endCollectionIndex] = ActionTextTMP;
            targetTMPCollection[endCollectionIndex] = target;
            toStringCollection[endCollectionIndex] = to;
            durationCollection[endCollectionIndex] = duration;
            startTimeCollection[endCollectionIndex] += delay;
            easeIdCollection[endCollectionIndex] = easeId;

            return currentTweenId;
        }

        private void ExecuteActionTextTMP(int index, bool isRequestComplete)
        {
            var startTime = startTimeCollection[index];

            if (Time.time < startTime && !isRequestComplete)
            {
                return;
            }

            var target = targetTMPCollection[index];

            workingCollection[index] = true;

            var delta = Time.time - startTime;
            var dulation = durationCollection[index];

            if (delta < dulation && !isRequestComplete)
            {
                var ratio = delta / dulation;
                var easeRatio = Ease(easeIdCollection[index], ratio);
                target.text = toStringCollection[index].Substring(0, (int)(toStringCollection[index].Length * easeRatio));
            }
            else
            {
                target.text = toStringCollection[index];

                UnregisterAction(index);
            }
        }

        /// <summary>
        /// AudioSourceコンポーネントのVolumeをTweenする
        /// </summary>
        /// <param name="target">tween対象</param>
        /// <param name="to">変更後の音量</param>
        /// <param name="duration">tweenにかける時間(s)</param>
        /// <param name="delay">tween開始を遅らせる時間(s)</param>
        /// <param name="easeId">イージング関数(tween.EaseXXX)</param>
        /// <returns>tweenId</returns>
        public int FadeVolumeTo(AudioSource target, float to, float duration, float delay, int easeId)
        {
            if (!ValidateRegisterAction()) return -1;

            RegisterAction();

            actionCollection[endCollectionIndex] = ActionFadeVolume;
            targetAudioSourceCollection[endCollectionIndex] = target;
            toVector3Collection[endCollectionIndex] = new Vector3(to, 0f);
            durationCollection[endCollectionIndex] = duration;
            startTimeCollection[endCollectionIndex] += delay;
            easeIdCollection[endCollectionIndex] = easeId;

            return currentTweenId;
        }

        private void ExecuteActionFadeVolume(int index, bool isRequestComplete)
        {
            var startTime = startTimeCollection[index];

            if (Time.time < startTime && !isRequestComplete)
            {
                return;
            }

            var target = targetAudioSourceCollection[index];

            if (!workingCollection[index])
            {
                workingCollection[index] = true;
                fromVector3Collection[index] = new Vector3(target.volume, 0f);
            }

            var delta = Time.time - startTime;
            var dulation = durationCollection[index];

            if (delta < dulation && !isRequestComplete)
            {
                var ratio = delta / dulation;
                var easeRatio = Ease(easeIdCollection[index], ratio);
                target.volume = Mathf.Lerp(fromVector3Collection[index].x, toVector3Collection[index].x, easeRatio);
            }
            else
            {
                target.volume = toVector3Collection[index].x;

                if (loopCountCollection[index] == 0)
                {
                    UnregisterAction(index);
                }
                else
                {
                    startTimeCollection[index] = startTime + dulation;

                    var loopMode = loopModeCollection[index];

                    if (loopMode == LoopModeRestart)
                    {
                        // Empty
                    }
                    else if (loopMode == LoopModeReverse)
                    {
                        var tmp = fromVector3Collection[index];
                        fromVector3Collection[index] = toVector3Collection[index];
                        toVector3Collection[index] = tmp;
                    }
                    else
                    {
                        UnregisterAction(index);
                    }

                    loopCountCollection[index] = Mathf.Max(-1, loopCountCollection[index] - 1);
                }
            }
        }

        /// <summary>
        /// 指定した時間後にGameObjectのSetActiveを変更する
        /// </summary>
        /// <param name="target"></param>
        /// <param name="active"></param>
        /// <param name="delay"></param>
        public int DelayedSetActive(GameObject target, bool active, float delay)
        {
            if (!ValidateRegisterAction()) return -1;

            RegisterAction();

            actionCollection[endCollectionIndex] = ActionDelayedSetActive;
            targetCollection[endCollectionIndex] = target;
            toBoolCollection[endCollectionIndex] = active;
            startTimeCollection[endCollectionIndex] += delay;

            return currentTweenId;
        }

        private void ExecuteDelayedSetActive(int index, bool isRequestComplete)
        {
            var startTime = startTimeCollection[index];

            if (Time.time < startTime && !isRequestComplete)
            {
                return;
            }

            targetCollection[index].SetActive(toBoolCollection[index]);

            UnregisterAction(index);
        }
        #endregion


        //======================================================================================
        // 外部公開用のユーティリティメソッド
        //======================================================================================
        #region public utility methods
        /// <summary>
        /// 動作中のtweenを完了状態にする
        /// </summary>
        /// <param name="id"></param>
        public void Complete(int tweenId)
        {
            var index = FindIndexById(tweenId);

            if (index < 0)
            {
                return;
            }

            ExecuteAction(index, true);
        }

        /// <summary>
        /// 動作中のtweenを中止する
        /// </summary>
        /// <param name="tweenId"></param>
        public void Kill(int tweenId)
        {
            var index = FindIndexById(tweenId);

            if (index < 0)
            {
                return;
            }

            UnregisterAction(index);
        }

        /// <summary>
        /// 始点を変えずにtweenを指定した回数繰り返す
        /// </summary>
        /// <param name="tweenId"></param>
        /// <param name="loops"></param>
        public void LoopRestart(int tweenId, int loops)
        {
            var index = FindIndexById(tweenId);

            if (index < 0)
            {
                return;
            }

            loopModeCollection[index] = LoopModeRestart;
            loopCountCollection[index] = loops;
        }

        /// <summary>
        /// 始点と終点を行き来するようにtweenを指定した回数繰り返す
        /// </summary>
        /// <param name="tweenId"></param>
        /// <param name="loops"></param>
        public void LoopReverse(int tweenId, int loops)
        {
            var index = FindIndexById(tweenId);

            if (index < 0)
            {
                return;
            }

            loopModeCollection[index] = LoopModeReverse;
            loopCountCollection[index] = loops;
        }

        /// <summary>
        /// 前回の終点を始点としてtweenを指定した回数繰り返す
        /// </summary>
        /// <param name="tweenId"></param>
        /// <param name="loops"></param>
        public void LoopIncremental(int tweenId, int loops)
        {
            var index = FindIndexById(tweenId);

            if (index < 0)
            {
                return;
            }

            loopModeCollection[index] = LoopModeIncremental;
            loopCountCollection[index] = loops;
        }
        #endregion


        //======================================================================================
        // 内部用のユーティリティメソッド
        //======================================================================================
        #region private utility methods
        /// <summary>
        /// アクション登録を検証する
        /// </summary>
        /// <returns></returns>
        private bool ValidateRegisterAction()
        {
            if (simultaneousSize <= numberOfTweening)
            {
                LogError("同時実行可能回数を超過しています");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 全てのアクションに共通の情報を登録する
        /// </summary>
        private void RegisterAction()
        {
            var index = endCollectionIndex;

            for (int i = 0; i < simultaneousSize; i++)
            {
                index = (index + 1) % simultaneousSize;
                
                if(tweenIdCollection[index] == 0)
                {
                    break;
                }
            }

            if (numberOfTweening == 0)
            {
                beginCollectionIndex = index;
            }
            else
            {
                previousIndexCollection[index] = endCollectionIndex;
                nextIndexCollection[endCollectionIndex] = index;
            }
            endCollectionIndex = index;

            currentTweenId++;
            numberOfTweening++;

            tweenIdCollection[index] = currentTweenId;
            workingCollection[index] = false;
            startTimeCollection[index] = Time.time;
            loopModeCollection[index] = LoopModeNone;
            loopCountCollection[index] = 0;
        }

        /// <summary>
        /// 登録されたアクションを削除する
        /// </summary>
        /// <param name="index"></param>
        private void UnregisterAction(int index)
        {
            var previousIndex = previousIndexCollection[index];
            var nextIndex = nextIndexCollection[index];

            if (numberOfTweening == 1)
            {
                beginCollectionIndex = -1;
                endCollectionIndex = -1;
            }
            else if (previousIndex != -1 && nextIndex != -1)
            {
                nextIndexCollection[previousIndexCollection[index]] = nextIndex;
                previousIndexCollection[nextIndexCollection[index]] = previousIndex;
            }
            else if (beginCollectionIndex == index)
            {
                beginCollectionIndex = nextIndexCollection[index];
                previousIndexCollection[nextIndexCollection[index]] = -1;
            }
            else if (endCollectionIndex == index)
            {
                endCollectionIndex = previousIndexCollection[index];
                nextIndexCollection[previousIndexCollection[index]] = -1;
            }
            previousIndexCollection[index] = -1;
            nextIndexCollection[index] = -1;

            numberOfTweening--;

            tweenIdCollection[index] = 0;
            actionCollection[index] = ActionNone;

            targetAudioSourceCollection[index] = null;
            targetCollection[index] = null;
            targetGraphicCollection[index] = null;
            targetAudioSourceCollection[index] = null;
            targetTextCollection[index] = null;
            targetTMPCollection[index] = null;
        }

        /// <summary>
        /// tweenIdからコレクションのindexを引き当てる
        /// </summary>
        /// <param name="tweenId"></param>
        /// <returns></returns>
        private int FindIndexById(int tweenId)
        {
            if (tweenId < 1)
            {
                LogError("引数" + nameof(tweenId) + "は1以上である必要があります");
                return -1;
            }

            if (numberOfTweening == 0) 
            { 
                return -1; 
            }

            int index = beginCollectionIndex;
            int result = -1;

            for (int i = 0; i < simultaneousSize; i++)
            {
                if (tweenIdCollection[index] == tweenId)
                {
                    result = index;
                    break;
                }

                index = nextIndexCollection[index];

                if (index == -1)
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// エラーログを出力する
        /// </summary>
        /// <param name="message"></param>
        private void LogError(object message)
        {
            Debug.LogError("[NukoTweenEngine]" + message);
        }
        #endregion


        //======================================================================================
        // イージング関数
        //======================================================================================
        #region Easing Functions
        private float Ease(int easeId, float x)
        {
            if (easeId == EaseLinear)
            {
                return EaseLinearFunction(x);
            }
            else if (easeId == EaseInSine)
            {
                return EaseInSineFunction(x);
            }
            else if (easeId == EaseOutSine)
            {
                return EaseOutSineFunction(x);
            }
            else if (easeId == EaseInOutSine)
            {
                return EaseInOutSineFunction(x);
            }
            else if (easeId == EaseInQuad)
            {
                return EaseInQuadFunction(x);
            }
            else if (easeId == EaseOutQuad)
            {
                return EaseOutQuadFunction(x);
            }
            else if (easeId == EaseInOutQuad)
            {
                return EaseInOutQuadFunction(x);
            }
            else if (easeId == EaseInCubic)
            {
                return EaseInCubicFunction(x);
            }
            else if (easeId == EaseOutCubic)
            {
                return EaseOutCubicFunction(x);
            }
            else if (easeId == EaseInOutCubic)
            {
                return EaseInOutCubicFunction(x);
            }
            else if (easeId == EaseInQuart)
            {
                return EaseInQuartFunction(x);
            }
            else if (easeId == EaseOutQuart)
            {
                return EaseOutQuartFunction(x);
            }
            else if (easeId == EaseInOutQuart)
            {
                return EaseInOutQuartFunction(x);
            }
            else if (easeId == EaseInQuint)
            {
                return EaseInQuintFunction(x);
            }
            else if (easeId == EaseOutQuint)
            {
                return EaseOutQuintFunction(x);
            }
            else if (easeId == EaseInOutQuint)
            {
                return EaseInOutQuintFunction(x);
            }
            else if (easeId == EaseInExpo)
            {
                return EaseInExpoFunction(x);
            }
            else if (easeId == EaseOutExpo)
            {
                return EaseOutExpoFunction(x);
            }
            else if (easeId == EaseInOutExpo)
            {
                return EaseInOutExpoFunction(x);
            }
            else if (easeId == EaseInCirc)
            {
                return EaseInCircFunction(x);
            }
            else if (easeId == EaseOutCirc)
            {
                return EaseOutCircFunction(x);
            }
            else if (easeId == EaseInOutCirc)
            {
                return EaseInOutCircFunction(x);
            }
            else if (easeId == EaseInBack)
            {
                return EaseInBackFunction(x);
            }
            else if (easeId == EaseOutBack)
            {
                return EaseOutBackFunction(x);
            }
            else if (easeId == EaseInOutBack)
            {
                return EaseInOutBackFunction(x);
            }
            else if (easeId == EaseInElastic)
            {
                return EaseInElasticFunction(x);
            }
            else if (easeId == EaseOutElastic)
            {
                return EaseOutElasticFunction(x);
            }
            else if (easeId == EaseInOutElastic)
            {
                return EaseInOutElasticFunction(x);
            }
            else if (easeId == EaseInBounce)
            {
                return EaseInBounceFunction(x);
            }
            else if (easeId == EaseOutBounce)
            {
                return EaseOutBounceFunction(x);
            }
            else if (easeId == EaseInOutBounce)
            {
                return EaseInOutBounceFunction(x);
            }
            else
            {
                return EaseLinearFunction(x);
            }
        }

        private float EaseLinearFunction(float x)
        {
            return x;
        }

        private float EaseInSineFunction(float x)
        {
            return 1f - Mathf.Cos((x * Mathf.PI) / 2f);
        }

        private float EaseOutSineFunction(float x)
        {
            return Mathf.Sin((x * Mathf.PI) / 2f);
        }

        private float EaseInOutSineFunction(float x)
        {
            return -(Mathf.Cos(Mathf.PI * x) - 1f) / 2f;
        }

        private float EaseInQuadFunction(float x)
        {
            return x * x;
        }

        private float EaseOutQuadFunction(float x)
        {
            return 1f - (1f - x) * (1f - x);
        }

        private float EaseInOutQuadFunction(float x)
        {
            return x < 0.5f ? 2f * x * x : 1f - Mathf.Pow(-2f * x + 2f, 2f) / 2f;
        }

        private float EaseInCubicFunction(float x)
        {
            return x * x * x;
        }

        private float EaseOutCubicFunction(float x)
        {
            return 1f - Mathf.Pow(1f - x, 3f);
        }

        private float EaseInOutCubicFunction(float x)
        {
            return x < 0.5f ? 4f * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
        }

        private float EaseInQuartFunction(float x)
        {
            return x * x * x * x;
        }

        private float EaseOutQuartFunction(float x)
        {
            return 1f - Mathf.Pow(1f - x, 4f);
        }

        private float EaseInOutQuartFunction(float x)
        {
            return x < 0.5f ? 8f * x * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 4f) / 2f;
        }

        private float EaseInQuintFunction(float x)
        {
            return x * x * x * x * x;
        }

        private float EaseOutQuintFunction(float x)
        {
            return 1f - Mathf.Pow(1f - x, 5f);
        }

        private float EaseInOutQuintFunction(float x)
        {
            return x < 0.5f ? 16f * x * x * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 5f) / 2f;
        }

        private float EaseInExpoFunction(float x)
        {
            return x == 0f ? 0f : Mathf.Pow(2f, 10f * x - 10f);
        }

        private float EaseOutExpoFunction(float x)
        {
            return x == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * x);
        }

        private float EaseInOutExpoFunction(float x)
        {
            return x == 0f ? 0f : x == 1f ? 1f : x < 0.5f ? Mathf.Pow(2f, 20f * x - 10f) / 2f : (2f - Mathf.Pow(2f, -20f * x + 10f)) / 2f;
        }

        private float EaseInCircFunction(float x)
        {
            return 1f - Mathf.Sqrt(1f - Mathf.Pow(x, 2f));
        }

        private float EaseOutCircFunction(float x)
        {
            return Mathf.Sqrt(1f - Mathf.Pow(x - 1f, 2f));
        }

        private float EaseInOutCircFunction(float x)
        {
            return x < 0.5f ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * x, 2f))) / 2f : (Mathf.Sqrt(1f - Mathf.Pow(-2f * x + 2f, 2f)) + 1f) / 2f;
        }

        private float EaseInBackFunction(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return c3 * x * x * x - c1 * x * x;
        }

        private float EaseOutBackFunction(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
        }

        private float EaseInOutBackFunction(float x)
        {
            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;
            return x < 0.5f ? (Mathf.Pow(2f * x, 2f) * ((c2 + 1f) * 2f * x - c2)) / 2f : (Mathf.Pow(2f * x - 2f, 2f) * ((c2 + 1f) * (x * 2f - 2f) + c2) + 2f) / 2f;
        }

        private float EaseInElasticFunction(float x)
        {
            const float c4 = (2f * Mathf.PI) / 3f;
            return x == 0f ? 0f : x == 1f ? 1f : -Mathf.Pow(2f, 10f * x - 10f) * Mathf.Sin((x * 10f - 10.75f) * c4);
        }

        private float EaseOutElasticFunction(float x)
        {
            const float c4 = (2f * Mathf.PI) / 3f;
            return x == 0f ? 0f : x == 1f ? 1f : Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * c4) + 1f;
        }

        private float EaseInOutElasticFunction(float x)
        {
            const float c5 = (2f * Mathf.PI) / 4.5f;
            return x == 0f ? 0f : x == 1f ? 1f : x < 0.5f ? -(Mathf.Pow(2f, 20f * x - 10f) * Mathf.Sin((20f * x - 11.125f) * c5)) / 2f : (Mathf.Pow(2f, -20f * x + 10f) * Mathf.Sin((20f * x - 11.125f) * c5)) / 2f + 1f;
        }

        private float EaseInBounceFunction(float x)
        {
            return 1f - EaseOutBounceFunction(1f - x);
        }

        private float EaseOutBounceFunction(float x)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;
            if (x < 1f / d1)
            {
                return n1 * x * x;
            }
            else if (x < 2f / d1)
            {
                return n1 * (x -= 1.5f / d1) * x + 0.75f;
            }
            else if (x < 2.5f / d1)
            {
                return n1 * (x -= 2.25f / d1) * x + 0.9375f;
            }
            else
            {
                return n1 * (x -= 2.625f / d1) * x + 0.984375f;
            }
        }

        private float EaseInOutBounceFunction(float x)
        {
            return x < 0.5f ? (1f - EaseOutBounceFunction(1f - 2f * x)) / 2f : (1f + EaseOutBounceFunction(2f * x - 1f)) / 2f;
        }
        #endregion
    }
}
