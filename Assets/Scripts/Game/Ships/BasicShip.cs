using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEditor.Tilemaps;
using UnityEngine;

public abstract class BasicShip : MonoBehaviour
{
    protected List<Command> commands = new();

    [SerializeField]
    protected float MaxVelocity = 5;

    [SerializeField]
    protected float MaxAngularVelocity = 1;


    protected abstract void SetCommands();

    // TODO:
    // This method should be removed
    // This is only here temporaily
    private void setCommands()
    {
        var body = GetComponent<Rigidbody2D>();

        // Added for better readability
        Func<bool> WKeyIsPressed = new(() => Input.GetKey(KeyCode.W));
        Func<bool> SKeyIsPressed = new(() => Input.GetKey(KeyCode.S));
        Func<bool> DKeyIsPressed = new(() => Input.GetKey(KeyCode.D));
        Func<bool> AKeyIsPressed = new(() => Input.GetKey(KeyCode.A));

        Func<bool> VelocityIsAboveMax = new(() => body.velocity.magnitude > MaxVelocity);
        Func<bool> AngularVelocityIsAboveMax = new(() => Mathf.Abs(body.angularVelocity) > MaxAngularVelocity);

        Func<bool> OnSlowVelocityKey = new(() => Input.GetKey(KeyCode.Space) && !(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)));
        Func<bool> OnSlowAngularVelocityKey = new(() => Input.GetKey(KeyCode.Space) && !(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)));

        // Regular movement controls
        commands.Add(new ThrustCommand(1f, this, WKeyIsPressed));
        commands.Add(new ThrustCommand(-1f, this, SKeyIsPressed));
        commands.Add(new TurnCommand(-0.1f, this, DKeyIsPressed));
        commands.Add(new TurnCommand(0.1f, this, AKeyIsPressed));

        // Counter force when max velocity exeeded
        commands.Add(new CounterThrustCommand(1f, body, VelocityIsAboveMax));
        commands.Add(new CounterTorqueCommand(0.1f, body, AngularVelocityIsAboveMax));


        // Make ship slow down
        commands.Add(new CounterThrustCommand(1f, body, OnSlowVelocityKey));
        commands.Add(new CounterTorqueCommand(0.1f, body, OnSlowAngularVelocityKey));

    }

    private void Start()
    {
        setCommands();
    }

    protected void FixedUpdate()
    {
        commands.ForEach(x => x.ExecuteIfCondition());
    }
}