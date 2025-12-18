using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour, IHealth
{
    [SerializeField] private float _maxHp = 100f;
    [SerializeField] private float _healEnergyCost;
    [SerializeField] private float _healAmount;
    [SerializeField] private float _healCooldown;

    private float _currentHp;
    private float _lastHealTime;

    public float CurrentHp => _currentHp;
    public float MaxHp => _maxHp;
    public bool IsDead => _currentHp <= 0;
    public bool CanHeal => Time.time >= _lastHealTime + _healCooldown;

    public event Action OnDeath;

    void Start ()
    {
        _currentHp = _maxHp;
    }

    public void Heal ( IEnergy energy )
    {
        if ( energy.CurrentEnergy >= _healEnergyCost && CanHeal )
        {
            energy.SpendEnergy ( _healEnergyCost, IEnergy.SpendMode.None );
            _currentHp = _currentHp + _healAmount > _maxHp ? _maxHp : _currentHp + _healAmount;
            _lastHealTime = Time.time;
        }
    }

    public void TakeDamage ( float damage )
    {
        _currentHp -= damage;
        if ( IsDead ) OnDeath?.Invoke ();
    }
}
