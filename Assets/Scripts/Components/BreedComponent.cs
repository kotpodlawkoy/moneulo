using UnityEngine;

public class BreedComponent : MonoBehaviour, IBreeder
{
    [SerializeField] private GameObject _childCell;
    [SerializeField] private float _breedEnergyCost;
    [SerializeField] private float _breedCooldown;
    [SerializeField] private float _energyToHealthRatio;
    [SerializeField] SetOfStatusEffects.StatusEffect breedSlowdown;

    private float _lastBreedTime;
    private IEnergy _energy;
    private IHealth _health;
    private IStatusEffectable _statusEffects;


    public bool CanBreed => Time.time >= _lastBreedTime + _breedCooldown;

    void Awake ()
    {
        _energy = GetComponent < IEnergy > ();
        _health = GetComponent < IHealth > ();
        _statusEffects = GetComponent < IStatusEffectable > ();
        _lastBreedTime = Time.time;
    }

    public GameObject Breed ()
    {
        if ( !CanBreed ) return null;
        
        _statusEffects?.SetStatusEffect ( breedSlowdown );
        _energy.SpendEnergy ( _breedEnergyCost, IEnergy.SpendMode.Forced, _health, _energyToHealthRatio );
        GameObject newCell = Instantiate ( _childCell, transform.position, transform.rotation );
        _lastBreedTime = Time.time;
        return newCell;
    }
}
