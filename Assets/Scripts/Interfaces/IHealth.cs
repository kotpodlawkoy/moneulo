using System;

public interface IHealth
{
    float CurrentHp { get; }
    float MaxHp { get; }

    bool IsDead { get; }
    bool CanHeal { get; } 
    event Action OnDeath;

    void TakeDamage ( float damage );
    void Heal ( IEnergy energy );
}
