using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Tilemaps;
using UnityEngine;


interface ICommand
{
    void Execute();
    //void SetCondition(Func<bool> func);
    bool ShouldExecute();
    void ExecuteIfCondition();
}

class MoveCommand : ICommand
{
    protected Func<bool> _conditionFunc;
    protected float _force;
    protected Rigidbody2D _body;


    public MoveCommand(float force, BasicShip ship, Func<bool> conditionFunc)
    {
        _force = force;
        _body = ship.GetComponent<Rigidbody2D>();
        _conditionFunc = conditionFunc;
    }

    public MoveCommand(float force, Rigidbody2D body, Func<bool> conditionFunc)
    {
        _force = force;
        _body = body;
        _conditionFunc = conditionFunc;
    }

    public bool ShouldExecute()
    {
        return _conditionFunc();
    }

    public virtual void Execute()
    {
        _body.AddRelativeForce(Vector2.up * _force);
    }

    public void ExecuteIfCondition()
    {
        if (ShouldExecute())
            Execute();
    }

}

class TurnCommand : MoveCommand
{
    public TurnCommand(float torque, BasicShip ship, Func<bool> conditionFunc) : base(torque, ship, conditionFunc) { }
    public TurnCommand(float torque, Rigidbody2D body, Func<bool> conditionFunc) : base(torque, body, conditionFunc) { }

    public override void Execute()
    {
        _body.AddTorque(_force);
    }
}

class CounterForceCommand : ICommand
{
    protected float _force;
    protected Rigidbody2D _body;
    protected Func<bool> _conditionFunc;

    public CounterForceCommand(float force, Rigidbody2D body, Func<bool> conditionFunc)
    {
        _force = force;
        _body = body;
        _conditionFunc = conditionFunc;
    }

    public virtual void Execute()
    {
        _body.AddForce(-_body.velocity.normalized * _force);

        if (_body.velocity.magnitude < 0.25f)
            _body.velocity = Vector2.zero;
    }

    public void ExecuteIfCondition()
    {
        if (_conditionFunc())
            Execute();
    }

    public bool ShouldExecute() => _conditionFunc();
}

class CounterTorqueCommand : CounterForceCommand
{
    public CounterTorqueCommand(float torque, Rigidbody2D body, Func<bool> conditionFunc) : base(torque, body, conditionFunc) { }

    public override void Execute()
    {
        _body.AddTorque(-Mathf.Sign(_body.angularVelocity)*_force);

        if (Mathf.Abs(_body.angularVelocity) < 2.5f)
            _body.angularVelocity = 0;
    }
}

// TODO:
// 

public class BasicShip : MonoBehaviour
{
    List<ICommand> commands = new();

    public float MaxVelocity = 5;
    public float MaxAngularVelocity = 1;

    private void Start()
    {
        var body = GetComponent<Rigidbody2D>();

        // Regular movement controls
        commands.Add(new MoveCommand(1f, this, new(() => Input.GetKey(KeyCode.W))));
        commands.Add(new MoveCommand(-1f, this, new(() => Input.GetKey(KeyCode.S))));
        commands.Add(new TurnCommand(-0.1f, this, new(() => Input.GetKey(KeyCode.D))));
        commands.Add(new TurnCommand(0.1f, this, new(() => Input.GetKey(KeyCode.A))));

        // Counter force when max velocity exeeded
        commands.Add(new CounterForceCommand(1f, body, new(() => body.velocity.magnitude > MaxVelocity)));
        commands.Add(new CounterTorqueCommand(0.1f, body, new(() => Mathf.Abs(body.angularVelocity) > MaxAngularVelocity)));

        // Make ship slow down
        commands.Add(new CounterForceCommand(1f, body,
            new
            (
                () => Input.GetKey(KeyCode.Space) && !(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            )));
        commands.Add(new CounterTorqueCommand(0.1f, body,
            new
            (
                () => Input.GetKey(KeyCode.Space) && !(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            )));
    }

    private void FixedUpdate()
    {
        commands.ForEach(x => x.ExecuteIfCondition());
    }

}
