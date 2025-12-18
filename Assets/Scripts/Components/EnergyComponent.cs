using UnityEngine;

public class EnergyComponent : MonoBehaviour, IEnergy
{
    [SerializeField] private float _maxEnergy = 100;
    private float _currentEnergy;
    
    public float MaxEnergy => _maxEnergy;
    public float CurrentEnergy => _currentEnergy;

    void Start ()
    {
        _currentEnergy = _maxEnergy;
    }

    public void SpendEnergy ( float energyAmount, IEnergy.SpendMode spendMode, IHealth health = null, float energyToHealthRatio = 1f )
    {
        if ( _currentEnergy - energyAmount < 0f )
        {
            energyAmount -= _currentEnergy;
            _currentEnergy = 0;
            if ( spendMode == IEnergy.SpendMode.Forced ) health.TakeDamage ( energyAmount * energyToHealthRatio );

        }
        else
        {
            _currentEnergy -= energyAmount;
        }
    }

    public void RestoreEnergy ( float energyAmount )
    {
        _currentEnergy = _currentEnergy + energyAmount > _maxEnergy ? _maxEnergy : _currentEnergy + energyAmount;
    }
}
