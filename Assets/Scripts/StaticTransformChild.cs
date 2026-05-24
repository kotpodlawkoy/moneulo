using UnityEngine;

public class StaticTransformChild : MonoBehaviour
{
    [ SerializeField ] private Vector3 _localOffset = new Vector3 ( 0, 0.7f, 0 );
    [ SerializeField ] private Vector3 _worldScale = Vector3.one;

    void LateUpdate ()
    {
        if ( transform.parent == null )
            return;

        transform.position = transform.parent.position + _localOffset;

        transform.rotation = Quaternion.identity;

        Vector3 parentScale = transform.parent.lossyScale;
        transform.localScale = new Vector3 (
            _worldScale.x / Mathf.Max ( 0.0001f, parentScale.x ),
            _worldScale.y / Mathf.Max ( 0.0001f, parentScale.y ),
            _worldScale.z / Mathf.Max ( 0.0001f, parentScale.z )
            );
    }
}
