using UnityEngine;
using System.Collections.Generic;

public class BaseVisionController : MonoBehaviour
{
    [Header ( "Vision settings" ) ]
    public float VisionRadius = 10f;
    [Range ( 0, 360 )] public float VisionAngle = 120f;
    public LayerMask targetLayers = ~0;
    public LayerMask obstacleLayers;
    
    [Header ( "Debug" ) ]
    public bool _showDebugGizmos = true;
    public Color gizmosColor = Color.blue;

    protected List < GameObject > visibleTargets = new List < GameObject > ();

    // Update is called once per frame
    void Update()
    {
        FindVisibleTargets ();
    }

    void FindVisibleTargets ()
    {
        visibleTargets.Clear ();
        Collider2D[] targetObjects = Physics2D.OverlapCircleAll ( transform.position, VisionRadius, targetLayers );

        foreach ( Collider2D target in targetObjects )
        {
            Vector2 directionToTarget = ( target.transform.position - transform.position ).normalized;
            float distanceToTarget = Vector2.Distance ( target.transform.position, transform.position );

            if ( Vector2.Angle ( transform.up, directionToTarget ) <= VisionAngle / 2 )
            {
                RaycastHit2D hit = Physics2D.Raycast ( transform.position, directionToTarget, distanceToTarget, obstacleLayers );
                Debug.DrawRay ( transform.position, new Vector3 ( directionToTarget.x,
                                                                   directionToTarget.y,
                                                                   0f ) * distanceToTarget );
                if ( !hit || target.transform.position == hit.collider.transform.position )
                {
                    visibleTargets.Add ( target.gameObject );
                }
            }
        }
    }

    public List < GameObject > GetVisibleTargets ()
    {
        return visibleTargets;
    }

    void OnDrawGizmosSelected ()
    {
        if ( !_showDebugGizmos )
        {
            return ;
        }

        Gizmos.color = gizmosColor;
        Gizmos.DrawWireSphere ( transform.position, VisionRadius );

        Vector3 rightCone = Quaternion.Euler ( 0, 0, -VisionAngle / 2 ) * transform.up; 
        Vector3 leftCone = Quaternion.Euler ( 0, 0, VisionAngle / 2 ) * transform.up; 

        Gizmos.DrawRay ( transform.position, rightCone * VisionRadius );
        Gizmos.DrawRay ( transform.position, leftCone * VisionRadius );
        
        Gizmos.color = Color.rebeccaPurple;
        foreach ( GameObject target in visibleTargets )
        {
            Gizmos.DrawLine ( transform.position, target.transform.position );
        }
    }

}
