using UnityEngine;

public abstract class CellController : MonoBehaviour
{
    [ SerializeField ] private GameObject meatPrefab;
    [ SerializeField ] private int meatAmount;

    [ SerializeField ] private float _age = 0f;
    private IHealth _health;
    private IEnergy _energy;
    private ILiving _living;
    private IMoveable _movement;
    private IAttacker _attacker;
    private IBreeder _breeder;
    private IGrowable _growable;
    private IEater _eater;
    private IStatusEffectable _statusEffects;
    private ISignal _signal;
    private IBrain _brain;

    public IBreeder Breeder => _breeder;
    public IGrowable Growable => _growable;
    public IHealth Health => _health;
    public IEnergy Energy => _energy;
    public IMoveable Movement => _movement;
    public ISignal SignalSource => _signal;
    public float Age => _age;

    public bool DebugLogging = false;
    public int CellId = 0;

    void Awake ()
    {
        _health = GetComponent<IHealth>();
        _energy = GetComponent<IEnergy>();
        _living = GetComponent<ILiving>();
        _movement = GetComponent<IMoveable>();
        _attacker = GetComponent<IAttacker>();
        _breeder = GetComponent<IBreeder>();
        _growable = GetComponent<IGrowable>();
        _eater = GetComponent<IEater>();
        _statusEffects = GetComponent<IStatusEffectable>();
        _signal = GetComponent<ISignal>();
        _brain = GetComponent<IBrain>();

        _health.OnDeath += OnDeathHandler;
    }

    void FixedUpdate ()
    {
        _age += Time.deltaTime;
    }

    void Update ()
    {
        _living.Live ();
        _statusEffects.UpdateStatusEffects ( Time.deltaTime );

        if ( _brain != null )
        {
            ApplyAction ( _brain.Decide ());
        }

        if ( DebugLogging )
            Debug.Log ( $"{gameObject.tag}: HP = {_health.CurrentHp} | ENERGY = {_energy.CurrentEnergy}" );
    }

    void ApplyAction ( CreatureAction a )
    {
        if ( a.MoveDirection.sqrMagnitude > 0.0001f )
        {
            if ( a.Run )
                _movement.StartRunning ();
            else
                _movement.StopRunning ();
            _movement.Move ( a.MoveDirection );
        }
        else
        {
            _movement.StopRunning ();
        }

        if ( a.Heal )
            _health.Heal ( _energy );
        if ( a.Breed && _breeder != null && _breeder.CanBreed )
            _breeder.Breed ();
        if ( a.Grow && _growable != null && _growable.CanGrow )
            _growable.Grow ();

        if ( _signal != null )
            _signal.SetSignal ( a.Signal );
    }

    void OnCollisionEnter2D ( Collision2D col )
    {
        if ( col.gameObject.CompareTag ( _eater.FoodTag ))
        {
            _eater.Eat ( col.gameObject );
        }
    }

    void OnCollisionStay2D ( Collision2D col )
    {
        if ( col.gameObject.CompareTag ( _attacker.VictimTag ) && !gameObject.CompareTag ( "Herbivore" ) )
        {
            _attacker.Attack ( col.gameObject );
        }
    }

    void OnDeathHandler ()
    {
        for ( int i = 0; i < meatAmount; i++ )
        {
            Instantiate ( meatPrefab, transform.position, transform.rotation );
        }
        Destroy ( gameObject );
    }

    public void Move ( Vector2 direction ) => _movement.Move ( direction );
    public void StartRunning () => _movement.StartRunning ();
    public void StopRunning () => _movement.StopRunning ();
    public void Heal () => _health.Heal ( _energy );
}
