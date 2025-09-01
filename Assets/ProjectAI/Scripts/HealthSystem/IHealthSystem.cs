public interface IHealthSystem
{
    int Health { get; }
    int MaxHealth { get; }
    void TakeDamage(int damage);
    void Heal(int healing);
    void Initialize(HealthModelsSO model);
    void ResetHealth();
}
