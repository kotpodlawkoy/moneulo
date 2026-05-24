using UnityEngine;

public class StatBarUI : MonoBehaviour
{
    public enum BarMode { Health, Energy, Signal }

    [Header("Mode")]
    [SerializeField] private BarMode _mode = BarMode.Health;

    [Header("Refs")]
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _fill;
    [SerializeField] private SpriteRenderer[] _renderers;

    [Header("Settings")]
    [SerializeField] private bool _showBar = true;
    [SerializeField] private float _barWidth = 1f;
    [SerializeField] private bool _hideWhenOffscreen = true;

    private IHealth _health;
    private IEnergy _energy;
    private ISignal _signal;
    private Camera _cam;

    void Start()
    {
        if (!_showBar) { gameObject.SetActive(false); return; }
        if (_target == null) _target = transform.root;

        switch (_mode)
        {
            case BarMode.Health: _health = _target.GetComponent<IHealth>(); break;
            case BarMode.Energy: _energy = _target.GetComponent<IEnergy>(); break;
            case BarMode.Signal: _signal = _target.GetComponent<ISignal>(); break;
        }

        _cam = Camera.main;
    }

    void LateUpdate()
    {
        if (_target == null) return;

        bool dataOk = (_mode == BarMode.Health) ? _health != null
                    : (_mode == BarMode.Energy) ? _energy != null
                    :                              _signal != null;
        if (!dataOk) { SetRenderersEnabled(false); return; }

        if (_hideWhenOffscreen && _cam != null)
        {
            Vector3 vp = _cam.WorldToViewportPoint(transform.position);
            bool visible = vp.z > 0 && vp.x > -0.05f && vp.x < 1.05f && vp.y > -0.05f && vp.y < 1.05f;
            SetRenderersEnabled(visible);
            if (!visible) return;
        }
        else
        {
            SetRenderersEnabled(true);
        }

        float ratio = 0f;
        switch (_mode)
        {
            case BarMode.Health:
                ratio = _health.CurrentHp / Mathf.Max(0.0001f, _health.MaxHp);
                break;
            case BarMode.Energy:
                ratio = _energy.CurrentEnergy / Mathf.Max(0.0001f, _energy.MaxEnergy);
                break;
            case BarMode.Signal:
                ratio = _signal.Signal;
                break;
        }
        ratio = Mathf.Clamp01(ratio);

        UpdateFill(ratio);
    }

    void UpdateFill(float ratio)
    {
        if (_fill == null) return;

        Vector3 s = _fill.localScale;
        s.x = _barWidth * ratio;
        _fill.localScale = s;

        Vector3 p = _fill.localPosition;
        p.x = -_barWidth * 0.5f + _barWidth * ratio * 0.5f;
    }

    void SetRenderersEnabled(bool enabled)
    {
        if (_renderers == null) return;
        foreach (var r in _renderers)
            if (r != null) r.enabled = enabled;
    }
}
