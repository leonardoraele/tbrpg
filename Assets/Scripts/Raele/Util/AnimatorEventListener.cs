using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Raele.Util
{
    /**
     * Remember the Animator states only emit callbacks
     */
    [SharedBetweenAnimators]
    public class AnimatorEventListener : StateMachineBehaviour
    {
        public struct StateIdentifier
        {
            public WeakReference<Animator> Animator;
            public int NameHash;
            public int LayerIndex;
        }

        private class StateIdentifierComparer : IEqualityComparer<StateIdentifier>
        {
            public static StateIdentifierComparer Instance =>
                s_instance ?? (s_instance = new StateIdentifierComparer());
            private static StateIdentifierComparer s_instance;

            public bool Equals(StateIdentifier arg0, StateIdentifier arg1)
                => arg0.NameHash == arg1.NameHash
                    && arg0.LayerIndex == arg1.LayerIndex
                    && arg0.Animator.TryGetTarget(out Animator anim0)
                    && arg1.Animator.TryGetTarget(out Animator anim1)
                    && anim0 == anim1;

            public int GetHashCode(StateIdentifier id)
                => id.NameHash;
        }

        public static Dictionary<StateIdentifier, List<Action>> OnStateEnterEvents =
            new Dictionary<StateIdentifier, List<Action>>(StateIdentifierComparer.Instance);

        // public static Dictionary<StateIdentifier, List<Action>> OnStateUpdateEvents =
        //     new Dictionary<StateIdentifier, List<Action>>(StateIdentifierComparer.Instance);

        public static Dictionary<StateIdentifier, List<Action>> OnStateExitEvents =
            new Dictionary<StateIdentifier, List<Action>>(StateIdentifierComparer.Instance);

        private IEnumerable<Dictionary<StateIdentifier, List<Action>>> AllDictionaries()
        {
            yield return OnStateEnterEvents;
            // yield return OnStateUpdateEvents;
            yield return OnStateExitEvents;
        }

        // Manually collects garbage because Unity is garbage so we have to do this shitty workaround to avoid memory leak
        private void Flush()
            => this.AllDictionaries()
                .ForEach(dictionary => {
                    dictionary.Keys
                        .Where(key => !key.Animator.TryGetTarget(out Animator a))
#if UNITY_EDITOR
                        .ThenIf(R.NotEmpty, keys => Debug.Log($"[AnimatorEventListeners] Removing {keys.ToArray().Length} listeners for destroyed Animators.")) // TODO Remove this
#endif
                        .ForEach(key => dictionary.Remove(key));
                });

        private void NotifyListeners(
            Dictionary<StateIdentifier, List<Action>> dictionary,
            Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex
        )
        {
            this.Flush();
            
            dictionary.GetOrDefault(new StateIdentifier() {
                    Animator = new WeakReference<Animator>(animator),
                    NameHash = stateInfo.shortNameHash,
                    LayerIndex = layerIndex,
                })
                ?.ForEach(R.SafeInvoke);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            => this.NotifyListeners(OnStateEnterEvents, animator, stateInfo, layerIndex);

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        // override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //     => this.NotifyListeners(OnStateUpdateEvents, animator, stateInfo, layerIndex);

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            => this.NotifyListeners(OnStateExitEvents, animator, stateInfo, layerIndex);

        // OnStateMove is called right after Animator.OnAnimatorMove()
        // override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //    // Implement code that processes and affects root motion
        // }

        // OnStateIK is called right after Animator.OnAnimatorIK()
        // override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //    // Implement code that sets up animation IK (inverse kinematics)
        // }
    }

    public static class AnimatorEventListenerHelper 
    {
        public static void AddStateEnterListener(this Animator animator, string stateShortName, int layerIndex, Action listener)
            => new AnimatorEventListener.StateIdentifier()
                    {
                        Animator = new WeakReference<Animator>(animator),
                        NameHash = Animator.StringToHash(stateShortName),
                        LayerIndex = layerIndex,
                    }
                .Then(identifier => AnimatorEventListener.OnStateEnterEvents
                    .GetOrInitialize(identifier, new List<Action>())
                )
                .Add(listener);

        public static void AddStateExitListener(this Animator animator, string stateShortName, int layerIndex, Action listener)
            => new AnimatorEventListener.StateIdentifier()
                    {
                        Animator = new WeakReference<Animator>(animator),
                        NameHash = Animator.StringToHash(stateShortName),
                        LayerIndex = layerIndex,
                    }
                .Then(identifier => AnimatorEventListener.OnStateExitEvents
                    .GetOrInitialize(identifier, new List<Action>())
                )
                .Add(listener);
    }
}
