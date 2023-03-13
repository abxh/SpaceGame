using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed class TurnCommand : Command
{
    private readonly float _torque;
    private readonly Rigidbody2D _body;

    public TurnCommand(float torque, BasicShip ship, Func<bool> conditionFunc) : base(conditionFunc)
    {
        _torque = torque;
        _body = ship.GetComponent<Rigidbody2D>();
    }

    public TurnCommand(float torque, Rigidbody2D body, Func<bool> conditionFunc) : base(conditionFunc)
    {
        _torque = torque;
        _body = body;
    }


    public override void Execute() =>
        _body.AddTorque(_torque);
}
