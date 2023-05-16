using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] private Vector3 Start;
    [SerializeField] private Vector3 End;

    [SerializeField] private List<LineEdge> _lineEdgePoints;

    
    private void OnDrawGizmos()
    {
        // Draw the line start and end.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Start, 0.1f);
        Gizmos.DrawWireSphere(End, 0.1f);
        
        // Draw the line.
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Start, End);
        
        // Draw points on line.
        if(_lineEdgePoints == null)
            return;

        //float lineLength = Vector3.Distance(Start, End);
        Vector3 previousInclinationLeft = Vector3.zero;
        Vector3 previousInclinationRight = Vector3.zero;
        foreach (LineEdge point in _lineEdgePoints)
        {
            // Flatten 3D points to a 2D plane, and calculate position in between.
            Vector3 posOnLine = GetPointOn3DLine(Start, End, point.DistanceFromStart);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(posOnLine, 0.1f);
            
            // Draw sister points.
            Vector3 leftInclination = GetInclinationPoint(posOnLine, End, point.RotationDegreesLeft, point.DistanceLeft);
            Vector3 rightInclination = GetInclinationPoint(posOnLine, End, point.RotationDegreesRight, point.DistanceRight);
            
            // Draw sister lines.
            Gizmos.color = Color.green;
            
            if(previousInclinationLeft != Vector3.zero)
                Gizmos.DrawLine(previousInclinationLeft, leftInclination);
            
            if(previousInclinationRight != Vector3.zero)
                Gizmos.DrawLine(previousInclinationRight, rightInclination);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(leftInclination, 0.05f);
            Gizmos.DrawWireSphere(rightInclination, 0.05f);

            previousInclinationLeft = leftInclination;
            previousInclinationRight = rightInclination;
        }
    }


    private static Vector3 GetPointOn3DLine(Vector3 start, Vector3 end, float distanceFromStart)
    {
        // Flatten points to 2D Plane.
        Vector2 flattenedStart = new Vector2(start.x, start.z);
        Vector2 flattenedEnd = new Vector2(end.x, end.z);

        // Calculate 0-1 percentage of position on flattened line.
        float flattenedLineLength = Vector2.Distance(flattenedStart, flattenedEnd);
        float factor = distanceFromStart / flattenedLineLength;

        // Get position on flattened line.
        Vector2 pointOnFlattenedLine = Vector2.Lerp(flattenedStart, flattenedEnd, factor);

        // Calculate height of flattened position.
        float heightIn3D = Mathf.Lerp(start.y, end.y, factor);

        // Construct a new point using flattened position and 3D height.
        return new Vector3(pointOnFlattenedLine.x, heightIn3D, pointOnFlattenedLine.y);
    }
    
    
    private Vector3 GetInclinationPoint(Vector3 parentPoint, Vector3 lineEnd, float rotationDegrees, float distance)
    {
        // Temporarily store rot and pos to current transform.
        transform.position = parentPoint;
        transform.LookAt(lineEnd);
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        eulerAngles.x = 0;
        eulerAngles.z = 0;
        transform.rotation = Quaternion.Euler(eulerAngles);
        
        // Rotate on Z.
        transform.Rotate(0, 0, rotationDegrees, Space.Self);
        
        // Calculate result point.
        Vector3 result = transform.position + transform.right * distance;

        // Clear temp pos and rot.
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        return result;
    }
}