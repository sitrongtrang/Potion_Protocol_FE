public interface ISpawnable<T> where T : ISpawnConfig
{
    void Initialize(T config);
}
