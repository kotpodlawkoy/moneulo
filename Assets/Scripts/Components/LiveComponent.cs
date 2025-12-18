using UnityEngine;

public class LiveComponent : MonoBehaviour, ILiving
{
    [SerializeField] private float _liveCooldown;
    [SerializeField] private float _liveEnergyCost;
    [SerializeField] private float _energyToHealthRatio;

    private IEnergy _energy;
    private IHealth _health;

    private float _lastLiveTime;

    public bool CanLive => Time.time >= _lastLiveTime + _liveCooldown;

    void Awake ()
    {
        _energy = GetComponent < IEnergy > ();
        _health = GetComponent < IHealth > ();
        _lastLiveTime = Time.time;
    }

    public void Live ()
    {
        if ( !CanLive ) return ;

        _energy.SpendEnergy ( _liveEnergyCost, IEnergy.SpendMode.Forced, _health, _energyToHealthRatio );
        _lastLiveTime = Time.time;
    }
}
