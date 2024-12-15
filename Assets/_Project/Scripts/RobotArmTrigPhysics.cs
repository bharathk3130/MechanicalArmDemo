using Clickbait.Utilities;
using UnityEngine;

[System.Serializable]
public class RobotArmTrigPhysics
{
    [SerializeField, NonEditable] float _targetDist;
    
    float _minRange, _maxRange;
    
    Arm _arm1, _arm2;
    Transform _base;
    Transform _target;
    Axis _axisToRotateBaseAround;

    float _theta1;

    Quaternion _baseRot;
    
    public RobotArmTrigPhysics(Arm arm1, Arm arm2, Transform target, Transform armBase, float minRange, float maxRange, 
        Axis axisToRotateBaseAround)
    {
        _arm1 = arm1;
        _arm2 = arm2;
        _base = armBase;
        _target = target;

        _minRange = minRange;
        _maxRange = maxRange;
        _axisToRotateBaseAround = axisToRotateBaseAround;
    }
    
    public void CalculateArmAngles()
    {
        CalculateDist();
        
        RotateBaseRightTowardsTarget();
        CalculateArm1();
        CalculateArm2();
    }
    
    void RotateBaseRightTowardsTarget()
    {
        Vector3 direction = _target.position - _base.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0)
        {
            float targetRotation = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            Quaternion rot;
            if (_axisToRotateBaseAround == Axis.X)
                rot = Quaternion.Euler(targetRotation, 0, 0);
            else if (_axisToRotateBaseAround == Axis.Y)
                rot = Quaternion.Euler(0, targetRotation, 0);
            else
                rot = Quaternion.Euler(0, 0, targetRotation);
            
            _baseRot = rot;
        }
    }

    public Quaternion GetBaseRotation() => _baseRot;
    
    void CalculateArm1()
    {
        float projection = _targetDist / 2;
        float cosTheta1 = Mathf.Clamp(projection / _arm1.Length, -1, 1);
        _theta1 = Mathf.Acos(cosTheta1) * Mathf.Rad2Deg;

        float targetHeight = _target.position.y - _arm1.Joint.position.y;
        float sinTheta0 = Mathf.Clamp(targetHeight / _targetDist, -1, 1);
        float theta0 = Mathf.Asin(sinTheta0) * Mathf.Rad2Deg;
        
        _arm1.Angle = _theta1 + theta0;
    }

    void CalculateArm2()
    {
        float projection = _targetDist / 2;
        float cosTheta3 = Mathf.Clamp(projection / _arm2.Length, -1, 1);
        float theta3 = Mathf.Acos(cosTheta3) * Mathf.Rad2Deg;

        // float theta = (90 - theta1) + (90 - _theta1);
        float theta = 180 - (theta3 + _theta1);
        _arm2.Angle = theta;
    }

    void CalculateDist() => 
        _targetDist = Mathf.Clamp(Vector3.Distance(_arm1.Joint.position, _target.position), _minRange, _maxRange);
}