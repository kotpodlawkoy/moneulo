using UnityEngine;

[RequireComponent ( typeof ( HealthComponent ) ) ]
[RequireComponent ( typeof ( EnergyComponent ) ) ]
[RequireComponent ( typeof ( LiveComponent ) ) ]
[RequireComponent ( typeof ( AttackComponent ) ) ]
[RequireComponent ( typeof ( EaterComponent ) ) ]
[RequireComponent ( typeof ( MovementComponent ) ) ]
[RequireComponent ( typeof ( BreedComponent ) ) ]
[RequireComponent ( typeof ( SetOfStatusEffects ) ) ]
public class AdultCellController : CellController
{
    public void Breed () => Breeder.Breed ();
}
