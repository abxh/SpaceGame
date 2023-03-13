using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float maxRange;
    public float maxAngle;
    public abstract void Shoot();
}
