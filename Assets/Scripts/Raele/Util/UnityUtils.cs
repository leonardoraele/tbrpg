using System;
using System.Collections.Generic;
using UnityEngine;

namespace Raele.Util
{
    public static class UnityUtils
    {
        public static Coroutine OnFinish(this YieldInstruction instruction, Action action)
            => UnityEventListener.Instance.StartCoroutine(UnityUtils.WaitInstruction(instruction, action));

        private static IEnumerator<YieldInstruction> WaitInstruction(YieldInstruction inst, Action action)
        {
            yield return inst;
            action?.SafeInvoke();
        }

        public static Coroutine OnFinish(this YieldInstruction instruction, Func<YieldInstruction> action)
            => UnityEventListener.Instance.StartCoroutine(UnityUtils.WaitInstruction(instruction, action));

        private static IEnumerator<YieldInstruction> WaitInstruction(YieldInstruction inst, Func<YieldInstruction> action)
        {
            yield return inst;
            yield return action?.SafeInvoke();
        }
    }
}