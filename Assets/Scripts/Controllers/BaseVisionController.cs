using UnityEngine;
using System.Collections.Generic;

public class BaseVisionController : MonoBehaviour
{
    public float VisionRadius = 10f;
    [Range(0, 360)] public float VisionAngle = 120f;
    public LayerMask targetLayers = ~0;
    public LayerMask obstacleLayers;

    [Range(3, 31)] public int rayCount = 11;

    [Header("Debug")]
    public bool _showDebugGizmos = true;
    public Color gizmosColor = Color.blue;

    protected List<GameObject> visibleTargets = new List<GameObject>();

    void Update()
    {
        FindVisibleTargets();
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider2D[] targetObjects = Physics2D.OverlapCircleAll(
            transform.position, VisionRadius, targetLayers);

        HashSet<GameObject> candidates = new HashSet<GameObject>();
        foreach (Collider2D target in targetObjects)
        {
            if (target.gameObject == gameObject) continue;
            candidates.Add(target.gameObject);
        }

        HashSet<GameObject> needRayCheck = new HashSet<GameObject>();
        foreach (var go in candidates)
        {
            var col = go.GetComponent<Collider2D>();
            if (col == null) continue;

            if (IsVisibleViaClosestPoint(col))
                visibleTargets.Add(go);
            else
                needRayCheck.Add(go);
        }

        if (needRayCheck.Count > 0)
            CheckViaRayFan(needRayCheck);
    }

    bool IsVisibleViaClosestPoint(Collider2D target)
    {
        Vector2 origin = transform.position;
        float halfFov = VisionAngle * 0.5f;

        Vector2 closest = target.ClosestPoint(origin);
        Vector2 toPoint = closest - origin;
        float dist = toPoint.magnitude;

        if (dist < 0.0001f) return true;
        if (dist > VisionRadius) return false;

        Vector2 dir = toPoint / dist;
        if (Vector2.Angle(transform.up, dir) > halfFov) return false;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, dist, obstacleLayers);
        if (hit.collider != null && hit.collider.gameObject != target.gameObject)
            return false;

        return true;
    }

    void CheckViaRayFan(HashSet<GameObject> candidates)
    {
        Vector2 origin = transform.position;
        Vector2 forward = transform.up;
        float halfFov = VisionAngle * 0.5f;
        LayerMask combinedMask = targetLayers | obstacleLayers;

        for (int i = 0; i < rayCount; i++)
        {
            float t = (rayCount == 1) ? 0.5f : (float)i / (rayCount - 1);
            float angle = Mathf.Lerp(-halfFov, halfFov, t);
            Vector2 dir = Quaternion.Euler(0, 0, angle) * forward;

            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, dir, VisionRadius, combinedMask);

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var hit in hits)
            {
                if (hit.collider == null) continue;
                if (hit.collider.gameObject == gameObject) continue;

                GameObject go = hit.collider.gameObject;

                if (candidates.Contains(go))
                {
                    if (!visibleTargets.Contains(go))
                        visibleTargets.Add(go);
                    break;
                }

                if (((1 << go.layer) & obstacleLayers) != 0)
                    break;
            }
        }
    }

    public List<GameObject> GetVisibleTargets() => visibleTargets;

    void OnDrawGizmosSelected()
    {
        if (!_showDebugGizmos) return;

        Gizmos.color = gizmosColor;
        Gizmos.DrawWireSphere(transform.position, VisionRadius);

        Vector3 rightCone = Quaternion.Euler(0, 0, -VisionAngle / 2) * transform.up;
        Vector3 leftCone  = Quaternion.Euler(0, 0,  VisionAngle / 2) * transform.up;
        Gizmos.DrawRay(transform.position, rightCone * VisionRadius);
        Gizmos.DrawRay(transform.position, leftCone  * VisionRadius);

        Gizmos.color = new Color(0.4f, 1f, 1f, 0.25f);
        for (int i = 0; i < rayCount; i++)
        {
            float t = (rayCount == 1) ? 0.5f : (float)i / (rayCount - 1);
            float angle = Mathf.Lerp(-VisionAngle / 2, VisionAngle / 2, t);
            Vector3 dir = Quaternion.Euler(0, 0, angle) * transform.up;
            Gizmos.DrawRay(transform.position, dir * VisionRadius);
        }

        if (Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            foreach (GameObject target in visibleTargets)
            {
                if (target == null) continue;
                var col = target.GetComponent<Collider2D>();
                if (col != null)
                {
                    Vector2 closest = col.ClosestPoint(transform.position);
                    Gizmos.DrawLine(transform.position, closest);
                }
            }
        }
    }
}
