using UnityEngine;

[RequireComponent ( typeof ( HealthComponent ) ) ]
[RequireComponent ( typeof ( EnergyComponent ) ) ]
[RequireComponent ( typeof ( LiveComponent ) ) ]
[RequireComponent ( typeof ( AttackComponent ) ) ]
[RequireComponent ( typeof ( EaterComponent ) ) ]
[RequireComponent ( typeof ( MovementComponent ) ) ]
[RequireComponent ( typeof ( GrowComponent ) ) ]
[RequireComponent ( typeof ( SetOfStatusEffects ) ) ]
public class ChildCellController : CellController
{
    public void Grow () => Growable.Grow ();
}
