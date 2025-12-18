using UnityEngine;
using System.Collections.Generic;

public class SetOfStatusEffects : MonoBehaviour, IStatusEffectable
{
    [System.Serializable]
    public class StatusEffect
    {
        [SerializeField] private string _name;
        [SerializeField] private float _duration;
        [SerializeField] private float _timer = 0f;
        [SerializeField] private float _multiplier;

        public string Name => _name;
        public float Duration => _duration;
        public float Timer => _timer;
        public float Multiplier => _multiplier;
        public bool IsActive => _timer > 0f;

        public void SetTimer () => _timer = _duration;

        public void UpdateEffect ( float deltaTime ) => _timer -= deltaTime;
    }

    [SerializeField] private List < StatusEffect > statusEffects = new ();

    public float GetSpeedMultiplier ()
    {
        float resultMultiplier = 1f;
        foreach ( StatusEffect effect in statusEffects )
        {
            resultMultiplier *= effect.Multiplier;
        }
        return resultMultiplier;
    }

    public void SetStatusEffect ( StatusEffect effect )
    {
        effect.SetTimer ();
        statusEffects.Add ( effect );
    }

    public void UpdateStatusEffects ( float deltaTime )
    {
        for ( int i = statusEffects.Count - 1; i >= 0; i -- )
        {
            statusEffects [ i ].UpdateEffect ( deltaTime );
            if ( !statusEffects [ i ].IsActive ) statusEffects.RemoveAt ( i );
        }
    }
}
