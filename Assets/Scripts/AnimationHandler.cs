using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Animator))]
public class AnimationHandler : MonoBehaviour
{
    private Animator _animator;
    private static readonly int Speed = Animator.StringToHash("speed");
    public float animationSpeedMultiplier = 0.1f;
    private static readonly int Jumping = Animator.StringToHash("jumping");
    public Transform bananaPosition;
    public Transform banana;
    public Transform hand;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Move(Vector3 speed)
    {
        _animator.SetFloat(Speed, Mathf.Sign(speed.y) * speed.magnitude * animationSpeedMultiplier);
        transform.forward = transform.parent.TransformDirection(speed.magnitude == 0 ? Vector3.forward : speed);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        _animator.SetIKPosition(AvatarIKGoal.RightHand, bananaPosition.position);
        _animator.SetIKRotation(AvatarIKGoal.RightHand, bananaPosition.rotation);
    }

    public void Jump()
    {
        _animator.SetBool(Jumping, true);
    }

    public void Land()
    {
        _animator.SetBool(Jumping, false);
    }
}
