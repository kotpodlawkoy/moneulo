using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(CellController))]
[RequireComponent(typeof(BaseVisionController))]
[RequireComponent(typeof(SignalComponent))]
public class CreatureAgent : Agent, IBrain
{
    [Header("Observation settings")]
    [SerializeField] private int _alliesObserved = 3;
    [SerializeField] private int _foodObserved = 3;
    [SerializeField] private int _enemiesObserved = 3;
    [SerializeField] private float _maxObsDistance = 10f;

    [Header("Tags for classification")]
    [SerializeField] private string _allySpeciesTag;
    [SerializeField] private string _foodTagToWatch;
    [SerializeField] private string _enemyTag;

    private CellController _cell;
    private BaseVisionController _vision;
    private ISignal _selfSignal;
    private CreatureAction _lastAction;

    protected void Awake ()
    {
        _cell = GetComponent < CellController > ();
        _vision = GetComponent < BaseVisionController > ();
        _selfSignal = GetComponent < ISignal > ();

        _cell.Health.OnDeath += OnDiedHandler;
    }

    public override void OnEpisodeBegin () {}

    public override void CollectObservations ( VectorSensor sensor )
    {
        sensor.AddObservation ( _cell.Health.CurrentHp / Mathf.Max ( 1f, _cell.Health.MaxHp ) );
        sensor.AddObservation ( _cell.Energy.CurrentEnergy / Mathf.Max(1f, _cell.Energy.MaxEnergy ) );
        sensor.AddObservation ( _selfSignal.Signal );
        float zRad = transform.eulerAngles.z * Mathf.Deg2Rad;
        sensor.AddObservation ( Mathf.Sin ( zRad ) );
        sensor.AddObservation ( Mathf.Cos ( zRad ) );

        var allies = new System.Collections.Generic.List < ( GameObject go, float dist, float angle ) > ();
        var foods = new System.Collections.Generic.List < ( GameObject go, float dist, float angle ) > ();
        var enemies = new System.Collections.Generic.List < ( GameObject go, float dist, float angle ) > ();

        Vector2 selfPos = transform.position;
        Vector2 selfUp = transform.up;

        foreach ( var t in _vision.GetVisibleTargets () )
        {
            if ( t == null ) continue;

            Vector2 to = ( Vector2 ) t.transform.position - selfPos;
            float dist = to.magnitude;
            float angle = Vector2.SignedAngle ( selfUp, to );

            if ( !string.IsNullOrEmpty ( _allySpeciesTag ) && t.CompareTag ( _allySpeciesTag ) )
                allies.Add ( ( t, dist, angle ) );
            if ( !string.IsNullOrEmpty ( _foodTagToWatch ) && t.CompareTag ( _foodTagToWatch ) )
                foods.Add ( ( t, dist, angle ) );
            if ( !string.IsNullOrEmpty ( _enemyTag ) && t.CompareTag ( _enemyTag ) )
                enemies.Add ( ( t, dist, angle ) );
        }

        allies.Sort ( ( a, b ) => a.dist.CompareTo ( b.dist ) );
        foods.Sort ( ( a, b ) => a.dist.CompareTo ( b.dist ) );
        enemies.Sort ( ( a, b ) => a.dist.CompareTo ( b.dist ) );

        for ( int i = 0; i < _alliesObserved; i++ )
        {
            if ( i < allies.Count )
            {
                var e = allies [ i ];
                var sig = e.go.GetComponent < ISignal > ();
                sensor.AddObservation ( sig != null ? sig.Signal : 0f );
                sensor.AddObservation ( Mathf.Clamp01 ( e.dist / _maxObsDistance ) );
                float rad = e.angle * Mathf.Deg2Rad;
                sensor.AddObservation ( Mathf.Sin ( rad ) );
                sensor.AddObservation ( Mathf.Cos ( rad ) );
            }
            else
            {
                sensor.AddObservation ( 0f );
                sensor.AddObservation ( 1f );
                sensor.AddObservation ( 0f );
                sensor.AddObservation ( 0f );
            }
        }

        for ( int i = 0; i < _foodObserved; i++ )
        {
            if ( i < foods.Count )
            {
                var e = foods [ i ];
                sensor.AddObservation ( Mathf.Clamp01 ( e.dist / _maxObsDistance ) );
                float rad = e.angle * Mathf.Deg2Rad;
                sensor.AddObservation ( Mathf.Sin ( rad ) );
                sensor.AddObservation ( Mathf.Cos ( rad ) ) ;
            }
            else { sensor.AddObservation ( 1f ) ; sensor.AddObservation ( 0f ) ; sensor.AddObservation ( 0f ) ; }
        }

        for ( int i = 0; i < _enemiesObserved; i++ ) 
        {
            if ( i < enemies.Count ) 
            {
                var e = enemies [ i ];
                sensor.AddObservation ( Mathf.Clamp01 ( e.dist / _maxObsDistance) ) ;
                float rad = e.angle * Mathf.Deg2Rad;
                sensor.AddObservation ( Mathf.Sin ( rad ) ) ;
                sensor.AddObservation ( Mathf.Cos ( rad ) ) ;
            }
            else { sensor.AddObservation ( 1f ) ; sensor.AddObservation ( 0f ) ; sensor.AddObservation ( 0f ) ; }
        }
    }

    public override void OnActionReceived ( ActionBuffers actions ) 
    {
        float dx = Mathf.Clamp ( actions.ContinuousActions [ 0 ] , -1f, 1f ) ;
        float dy = Mathf.Clamp ( actions.ContinuousActions [ 1 ] , -1f, 1f ) ;
        float sig = ( Mathf.Clamp ( actions.ContinuousActions [ 2 ] , -1f, 1f ) + 1f ) * 0.5f;

        bool run = actions.DiscreteActions [ 0 ] == 1;
        bool heal = actions.DiscreteActions [ 1 ] == 1;
        bool act = actions.DiscreteActions [ 2 ] == 1;

        Vector2 move = new Vector2 ( dx, dy ) ;
        if ( move.sqrMagnitude < 0.01f ) move = Vector2.zero;

        _lastAction = new CreatureAction
        {
            MoveDirection = move,
            Run = run,
            Heal = heal,
            Breed = act,
            Grow = act,
            Signal = sig
        };
    }

    public CreatureAction Decide ( ) 
    {
        return _lastAction;
    }

    public override void Heuristic ( in ActionBuffers actionsOut ) 
    {
        var c = actionsOut.ContinuousActions;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint ( Input.mousePosition ) ;
        Vector2 toMouse = ( Vector2 ) mouseWorld - ( Vector2 ) transform.position;
        if ( toMouse.magnitude > 0.1f ) 
        {
            if ( Input.GetKey ( KeyCode.W ) ) 
            {
                Vector2 n = toMouse.normalized;
                c [ 0 ] = n.x; c [ 1 ] = n.y;
            }
            else { c [ 0 ] = 0; c [ 1 ] = 0; }
        }
        c [ 2 ] = Input.GetKey ( KeyCode.Q ) ? 1f : -1f;

        var d = actionsOut.DiscreteActions;
        d [ 0 ] = Input.GetKey ( KeyCode.LeftShift ) ? 1 : 0;
        d [ 1 ] = Input.GetKey ( KeyCode.Space ) ? 1 : 0;
        d [ 2 ] = ( Input.GetKeyDown ( KeyCode.B ) || Input.GetKeyDown ( KeyCode.G ) ) ? 1 : 0;
    }

    public void RewardAteFood ( float currentEnergy, float maxEnergy ) 
    {
        AddReward ( 1f * ( 1f - currentEnergy / maxEnergy ) ) ;
    }
    public void RewardAttackedPrey () { AddReward ( 0.5f ) ; }
    public void RewardBred () { AddReward ( 3.0f ) ; }
    public void RewardGrew () { AddReward ( 1.5f ) ; }
    public void RewardTookDamage ( float dmg, float maxHp ) {}

    void OnDiedHandler () 
    {
        AddReward ( -10.0f ) ;
        EndEpisode () ;
    }
}
