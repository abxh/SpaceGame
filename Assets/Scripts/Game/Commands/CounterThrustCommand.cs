using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed class CounterThrustCommand : Command
{
    private readonly float _force;
    private readonly Rigidbody2D _body;

    public CounterThrustCommand(float force, Rigidbody2D body, Func<bool> conditionFunc) : base(conditionFunc)
    {
        _force = force;
        _body = body;
    }

    public override void Execute()
    {
        _body.AddForce(-_body.velocity.normalized * _force);

        if (_body.velocity.magnitude < 0.25f)
            _body.velocity = Vector2.zero;
    }
}
