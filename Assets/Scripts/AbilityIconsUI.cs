using UnityEngine;

public class AbilityIconsUI : MonoBehaviour
{
    [System.Serializable]
    public class IconEntry
    {
        public enum AbilityType { Breed, Grow, Heal, Attack }
        public AbilityType type;
        public SpriteRenderer icon;
    }

    [Header("Refs")]
    [SerializeField] private Transform _target;
    [SerializeField] private IconEntry [] _icons;

    [Header("Settings")]
    [SerializeField] private bool _showIcons = true;
    [SerializeField] private bool _hideWhenOffscreen = true;

    private IBreeder _breeder;
    private IGrowable _growable;
    private IHealth _health;
    private IEnergy _energy;
    private IAttacker _attacker;
    private Camera _cam;

    void Start ()
    {
        if ( !_showIcons ) { gameObject.SetActive ( false ); return; }
        if ( _target == null ) _target = transform.root;

        _breeder  = _target.GetComponent<IBreeder> ();
        _growable = _target.GetComponent<IGrowable> ();
        _health   = _target.GetComponent<IHealth> ();
        _energy   = _target.GetComponent<IEnergy> ();
        _attacker = _target.GetComponent<IAttacker> ();

        _cam = Camera.main;

        foreach ( var e in _icons )
            if ( e.icon != null ) e.icon.enabled = false;
    }

    void LateUpdate ()
    {
        if ( _target == null ) return;

        if ( _hideWhenOffscreen && _cam != null )
        {
            Vector3 vp = _cam.WorldToViewportPoint ( transform.position );
            bool visible = vp.z > 0 && vp.x > -0.05f && vp.x < 1.05f && vp.y > -0.05f && vp.y < 1.05f;
            if ( !visible )
            {
                foreach ( var e in _icons )
                    if ( e.icon != null ) e.icon.enabled = false;
                return;
            }
        }

        foreach ( var e in _icons )
        {
            if ( e.icon == null ) continue;
            e.icon.enabled = IsAbilityReady ( e.type );
        }
    }

    bool IsAbilityReady ( IconEntry.AbilityType type )
    {
        switch ( type )
        {
            case IconEntry.AbilityType.Breed:
                return _breeder != null && _breeder.CanBreed;
            case IconEntry.AbilityType.Grow:
                return _growable != null && _growable.CanGrow;
            case IconEntry.AbilityType.Heal:
                return _health != null && _health.CanHeal;
            case IconEntry.AbilityType.Attack:
                return _attacker != null && _attacker.CanAttack;
        }
        return false;
    }
}
