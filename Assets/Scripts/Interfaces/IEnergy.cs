public interface IEnergy
{
    public enum SpendMode { None, Forced };

    float CurrentEnergy { get; }
    float MaxEnergy { get; }

    void SpendEnergy ( float energyAmount, SpendMode spendMode, IHealth health = null, float energyToHealthRatio = 1f );
    void RestoreEnergy ( float energyAmount );
}
