using System;
using UnityEngine;

public class BoomAnimation : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!_animator.isActiveAndEnabled) BoomEnd();
    }

    private void BoomEnd() => Destroy(gameObject);
}