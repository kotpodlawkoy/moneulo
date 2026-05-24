using UnityEngine;

public class SignalComponent : MonoBehaviour, ISignal
{
    [ SerializeField, Range ( 0f, 1f ) ] private float _signal = 0f;

    [ SerializeField ] private float _decayPerSecond = 0f;

    public float Signal => _signal;

    public void SetSignal ( float value )
    {
        _signal = Mathf.Clamp01 ( value );
    }

    void Update ()
    {
        if ( _decayPerSecond > 0f && _signal > 0f )
        {
            _signal = Mathf.Max ( 0f, _signal - _decayPerSecond * Time.deltaTime );
        }
    }
}
