using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed class ThrustCommand : Command
{
    private readonly float _force;
    private readonly Rigidbody2D _body;

    public ThrustCommand(float force, BasicShip ship, Func<bool> conditionFunc)
    {
        _force = force;
        _body = ship.GetComponent<Rigidbody2D>();
        conditionFunction = conditionFunc;
    }

    public ThrustCommand(float force, Rigidbody2D body, Func<bool> conditionFunc)
    {
        _force = force;
        _body = body;
        conditionFunction = conditionFunc;
    }

    public override void Execute() =>
        _body.AddRelativeForce(Vector2.up * _force);

}
