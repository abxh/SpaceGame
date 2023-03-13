using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed class CounterTorqueCommand : Command
{
    private readonly float _torque;
    private readonly Rigidbody2D _body;

    public CounterTorqueCommand(float torque, Rigidbody2D body, Func<bool> conditionFunc) : base(conditionFunc)
    {
        _torque = torque;
        _body = body;
    }

    public override void Execute()
    {
        _body.AddTorque(-Mathf.Sign(_body.angularVelocity) * _torque);

        if (Mathf.Abs(_body.angularVelocity) < 2.5f)
            _body.angularVelocity = 0;
    }
}