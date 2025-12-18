using UnityEngine;
using System.Collections;

public class GrowComponent : MonoBehaviour, IGrowable
{
    [SerializeField] private GameObject _adultCell;
    [SerializeField] private float _growEnergyCost;
    [SerializeField] private float _growCooldown;
    [SerializeField] private float _energyToHealthRatio;

    private float _lastGrowTime;
    private IEnergy _energy;
    private IHealth _health;

    public bool CanGrow => Time.time >= _lastGrowTime + _growCooldown;

    void Awake ()
    {
        _energy = GetComponent < IEnergy > ();
        _health = GetComponent < IHealth > ();
        _lastGrowTime = Time.time;
    }

    public GameObject Grow ()
    {
        if ( !CanGrow ) return null;
        
        _energy.SpendEnergy ( _growEnergyCost, IEnergy.SpendMode.Forced, _health, _energyToHealthRatio );
        GameObject grownCell = Instantiate ( _adultCell, transform.position, transform.rotation );
        StartCoroutine ( TransferState ( grownCell.GetComponent < CellController > () ) );
        _lastGrowTime = Time.time;
        return grownCell;
    }

    IEnumerator TransferState ( CellController cellController )
    {
        yield return new WaitForEndOfFrame ();

        IHealth otherHealth = cellController.GetComponent < IHealth > ();
        IEnergy otherEnergy = cellController.GetComponent < IEnergy > ();

        otherHealth.TakeDamage  ( otherHealth.MaxHp * ( 1f - _health.CurrentHp / _health.MaxHp ) );
        otherEnergy.SpendEnergy ( otherEnergy.MaxEnergy * ( 1f - _energy.CurrentEnergy / _energy.MaxEnergy ), IEnergy.SpendMode.None );
        Debug.LogError ( $"hp = {otherHealth.CurrentHp}, energy = {otherEnergy.CurrentEnergy}!" );

        Destroy ( gameObject );
    }
}
