using System;
using UnityEngine;

[Serializable]
public struct LineEdge
{
    public float DistanceFromStart;
    
    // Point on left side.
    public float DistanceLeft;
    public float RotationDegreesLeft;
    
    // Point on right side.
    public float DistanceRight;
    public float RotationDegreesRight;
}