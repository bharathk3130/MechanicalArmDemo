using System;
using Clickbait.Utilities;
using UnityEngine;

public class RoboticArm : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _base;
    [SerializeField] Arm _arm1;
    [SerializeField] Arm _arm2;
    [SerializeField] Transform _target;

    [Header("Settings")]
    [SerializeField] Axis _axisToRotateBaseAround = Axis.Y;
    [SerializeField] float _minRange = 1;
    
    [Header("Calculations")]
    // Only to view fields in the inspector for debugging purposes. Changes made outside play mode will get overriden
    [SerializeField] RobotArmTrigPhysics _robotArmPhysics;

    float _maxRange;

    void Awake()
    {
        _arm1.Length = Vector3.Distance(_arm1.Joint.position, _arm1.Tip.position);
        _arm2.Length = Vector3.Distance(_arm2.Joint.position, _arm2.Tip.position);
        
        _maxRange = _arm1.Length + _arm2.Length;
        
        _robotArmPhysics = new RobotArmTrigPhysics(_arm1, _arm2, _target, _base, _minRange, _maxRange, _axisToRotateBaseAround);
    }

    void Update()
    {
        _robotArmPhysics.CalculateArmAngles();
        
        RotateArm();
        RotateBase(_robotArmPhysics.GetBaseRotation());
    }

    void RotateArm()
    {
        _arm1.Joint.localEulerAngles = new Vector3(0, 0, _arm1.Angle);
        _arm2.Joint.localEulerAngles = new Vector3(0, 0, _arm2.Angle);
    }
    
    void RotateBase(Quaternion rot) => _base.localRotation = rot;
}

[Serializable]
public class Arm
{
    public Transform Joint;
    public Transform Tip;
    public float Angle;
    [NonEditable] public float Length;
}

public enum Axis
{
    X,
    Y,
    Z
}