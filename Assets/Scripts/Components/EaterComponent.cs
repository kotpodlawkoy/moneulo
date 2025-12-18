using UnityEngine;

public class EaterComponent : MonoBehaviour, IEater
{
    [SerializeField] private float _eatAmount;    
    [SerializeField] private float _eatWealth;    
    [SerializeField] private float _eatCooldown;    
    [SerializeField] private string _foodTag;    
    [SerializeField] private SetOfStatusEffects.StatusEffect _eatSlowdown;

    private float _lastEatTime;
    private IEnergy _energy; 
    private IStatusEffectable _statusEffects;
    
    public bool CanEat => Time.time >= _lastEatTime + _eatCooldown;
    public string FoodTag => _foodTag;

    void Awake ()
    {
        _energy = GetComponent < IEnergy > ();
        _statusEffects = GetComponent < IStatusEffectable > ();
        _lastEatTime = Time.time;
    }

    public void Eat ( GameObject food )
    {
        if ( !CanEat ) return ;

        _energy.RestoreEnergy ( _eatWealth );
        food.GetComponent < FoodController > ().GetEaten ( _eatAmount );
        _statusEffects.SetStatusEffect ( _eatSlowdown );
        _lastEatTime = Time.time;
    } 
}
