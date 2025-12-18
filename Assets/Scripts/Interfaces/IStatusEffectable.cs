public interface IStatusEffectable
{
    float GetSpeedMultiplier ();

    void SetStatusEffect ( SetOfStatusEffects.StatusEffect effect );
    void UpdateStatusEffects ( float deltaTime );
}
