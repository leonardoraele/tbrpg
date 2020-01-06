using System.Collections.Generic;
using UnityEngine;
using Raele.Util;

namespace Raele.TBRPG.View
{
    public class Actor : MonoBehaviour
    {
        public string IdleAnimation;
        public string PrepareActionAnimation;
        public string GetHitAnimation;
        public string AttackAnimation;
        public string DefendAnimation;
        public string MoveAnimation;
        public string DieAnimation;

        private Animation Animation;

        public void Awake()
            => this.Animation = this.gameObject.GetComponent<Animation>()
                .AssertNotDefault();

        public void Start()
            => this.Animation.Play(this.IdleAnimation);

        public YieldInstruction PlayIdle()
            => this.PlayAndWait(this.IdleAnimation);

        public YieldInstruction PlayPrepareAction()
            => this.PlayAndWait(this.PrepareActionAnimation);

        public YieldInstruction PlayGetHit()
            => this.PlayAndWait(this.GetHitAnimation);

        public YieldInstruction PlayAttack()
            => this.PlayAndWait(this.AttackAnimation);
        
        public YieldInstruction PlayDefend()
            => this.PlayAndWait(this.DefendAnimation);
        
        public YieldInstruction PlayMove()
            => this.PlayAndWait(this.MoveAnimation);
        
        public YieldInstruction PlayDie()
            => this.PlayAndWait(this.DieAnimation);

        private YieldInstruction PlayAndWait(string animationName)
            => string.IsNullOrEmpty(animationName)
                   .ThenDo(() => Debug.LogWarning($"Tried to play animation for actor {this.gameObject.name} but provided animation name was null or empty."))
                   .Then(() => new WaitForEndOfFrame())
               ?? this.Animation[animationName]
                   .AssertNotDefault(() => $"Actor {this.gameObject.name} doesn't have an animation named {animationName} in it's Animation component.")
                   .As(out AnimationState animInfo)
                   .Then(() => this.Animation.Play(animationName))
                   .Then<bool, YieldInstruction>(() => new WaitForSeconds(animInfo.length))
                   .OtherwiseThrow(() => $"Actor {this.gameObject.name} couldn't play animation {animationName}.");
    }
}
