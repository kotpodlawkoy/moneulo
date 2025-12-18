using UnityEngine;

public class AttackComponent : MonoBehaviour, IAttacker
{
    [SerializeField] private float _attackDamage;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _attackEnergyCost;
    [SerializeField] private float _energyToHealthRatio;
    [SerializeField] private string _victimTag;

    private IEnergy _energy;
    private IHealth _health;

    private float _lastAttackTime;

    public float AttackDamage => _attackDamage;
    public string VictimTag => _victimTag;
    public bool CanAttack => Time.time >= _lastAttackTime + _attackCooldown;

    void Awake ()
    {
        _energy = GetComponent < IEnergy > ();
        _health = GetComponent < IHealth > ();
        _lastAttackTime = Time.time;
    }

    public void Attack ( GameObject target )
    {
        if ( !CanAttack ) return ;

        target.GetComponent < HealthComponent > ().TakeDamage ( _attackDamage );
        _energy.SpendEnergy ( _attackEnergyCost, IEnergy.SpendMode.Forced, _health, _energyToHealthRatio );
        _lastAttackTime = Time.time;
    }
}
