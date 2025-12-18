using UnityEngine;

public interface IAttacker
{
    float AttackDamage { get; }
    bool CanAttack { get; }
    string VictimTag { get; }
    void Attack ( GameObject target );
}
