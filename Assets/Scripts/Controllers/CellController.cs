using UnityEngine;

public class CellController : MonoBehaviour
{
    [SerializeField] private GameObject meatPrefab;
    [SerializeField] private int meatAmount;

    private IHealth _health;
    private IEnergy _energy;
    private ILiving _living;
    private IMoveable _movement;
    private IAttacker _attacker;
    private IBreeder _breeder;
    private IGrowable _growable;
    private IEater _eater;
    private IStatusEffectable _statusEffects;

    public IBreeder Breeder => _breeder;
    public IGrowable Growable => _growable;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake ()
    {
        
        _health = GetComponent < IHealth > ();
        _energy = GetComponent < IEnergy > ();
        _living = GetComponent < ILiving > ();
        _movement = GetComponent < IMoveable > ();
        _attacker = GetComponent < IAttacker > ();
        _breeder = GetComponent < IBreeder > ();
        _growable = GetComponent < IGrowable > ();
        _eater = GetComponent < IEater > ();
        _statusEffects = GetComponent < IStatusEffectable > ();

        _health.OnDeath += OnDeathHandler;
    }

    // Update is called once per frame
    void Update ()
    {
        _living.Live ();
        _statusEffects.UpdateStatusEffects ( Time.deltaTime );
        Debug.Log ( $"{gameObject.tag}: HP = {_health.CurrentHp} | ENERGY = {_energy.CurrentEnergy}" );
    }

    void OnCollisionEnter2D ( Collision2D col )
    {
        if ( col.gameObject.CompareTag ( _eater.FoodTag ) )
        {
            _eater.Eat ( col.gameObject );
        }
    }

    void OnCollisionStay2D ( Collision2D col )
    {
        if ( col.gameObject.CompareTag ( _attacker.VictimTag ) )
        {
            _attacker.Attack ( col.gameObject );
        }
    }

    void OnDeathHandler ()
    {
        for ( int i = 0; i < meatAmount; i ++ ) Instantiate ( meatPrefab, transform.position, transform.rotation );
        Destroy ( gameObject );
    }

    public void Move ( Vector2 direction ) => _movement.Move ( direction );
    public void StartRunning () => _movement.StartRunning ();
    public void StopRunning () => _movement.StopRunning ();
    public void Heal () => _health.Heal ( _energy );
}
