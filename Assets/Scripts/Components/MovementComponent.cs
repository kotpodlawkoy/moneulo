using UnityEngine;

public class MovementComponent : MonoBehaviour, IMoveable
{
    [SerializeField] private float _baseSpeed;
    [SerializeField] private float _baseEnergyCostPerDist;
    [SerializeField] private float _runSpeedMultiplier;
    [SerializeField] private float _runCostMultiplier;
    [SerializeField] private float _energyToHealthRatio;

    private IEnergy _energy;
    private IHealth _health;
    private IStatusEffectable _statusEffects;

    private bool _isRunning = false;

    public float Speed { get; private set; }
    public bool IsRunning => _isRunning;

    void Awake ()
    {
        _energy = GetComponent < IEnergy > ();
        _health = GetComponent < IHealth > ();
        _statusEffects = GetComponent < IStatusEffectable > ();
    }

    void Update ()
    {
        float speedMultiplier = _statusEffects?.GetSpeedMultiplier () ?? 1f;
        float currentSpeed = _baseSpeed * speedMultiplier;
        if ( _isRunning ) currentSpeed *= _runSpeedMultiplier;
        Speed = currentSpeed;
    }

    public void Move ( Vector2 direction )
    {
        float energyCost = _baseEnergyCostPerDist * Speed * Time.deltaTime;
        if ( _isRunning ) energyCost *= _runCostMultiplier;
        _energy.SpendEnergy ( energyCost, IEnergy.SpendMode.Forced, _health, _energyToHealthRatio );

        float rotationAngle = Vector2.SignedAngle ( Vector2.up,
                                                    direction );
        transform.rotation = Quaternion.Euler ( 0f, 0f, rotationAngle );
        transform.Translate ( Vector2.up * Speed * Time.deltaTime );
    }

    public void StartRunning () => _isRunning = true;
    public void StopRunning () => _isRunning = false;
}

