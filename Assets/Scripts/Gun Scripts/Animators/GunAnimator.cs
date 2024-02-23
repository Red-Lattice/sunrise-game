using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static AnimationState;
using static System.Enum;

public enum AnimationState {
    Enter,
    Fire,
    Exit,
    Punch,
    Idle,
    Running,
    None,
    Wall,
}

public static class GunAnimator
{
    // State changing methods

    /// <summary>
    /// Plays an animation with a gun based on the info passed through the parameter struct.
    /// </summary>
    /// <returns>The state of the animation after the function has run</returns>
    public static void PlayAnimation(AnimationInfo animationInfo) {
        if (!Interruptable(animationInfo.activeAnimation)) {return;}

        animationInfo.gunAnimator.Play(animationInfo.animation.ToString());
    }
    
    // Pure functions
    static bool Interruptable(AnimationState activeAnimation) {
        switch (activeAnimation) {
            case Idle:
            case None:
                return true;
            default:
                return false;
        }
    }

    public static AnimationState FindAnimationState(Animator animator) {
        AnimationState state = None;
        var animatorClips = animator.GetCurrentAnimatorClipInfo(0);

        if (animatorClips.Length > 0) {TryParse(animatorClips[0].clip.name, out state);}

        return state;
    }
}

public struct AnimationInfo {
    public AnimationState animation;
    public Animator gunAnimator;
    public AnimationState activeAnimation;

    public AnimationInfo(AnimationState animation, Animator gunAnimator, AnimationState activeAnimation) {
        this.animation = animation;
        this.gunAnimator = gunAnimator;
        this.activeAnimation = activeAnimation;
    }
}