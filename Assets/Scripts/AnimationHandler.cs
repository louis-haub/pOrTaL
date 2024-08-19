using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationHandler : MonoBehaviour
{
    private Animator _animator;
    private static readonly int Speed = Animator.StringToHash("speed");
    public float animationSpeedMultiplier = 0.1f;
    private static readonly int Jumping = Animator.StringToHash("jumping");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Move(Vector3 speed)
    {
        _animator.SetFloat(Speed, Mathf.Sign(speed.y) * speed.magnitude * animationSpeedMultiplier);
        transform.forward = transform.parent.TransformDirection(speed.magnitude == 0 ? Vector3.forward : speed);
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
