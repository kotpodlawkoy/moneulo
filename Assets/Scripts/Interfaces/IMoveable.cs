using UnityEngine;

public interface IMoveable
{
    float Speed { get; }
    bool IsRunning { get; }

    void Move ( Vector2 direction );
    void StartRunning ();
    void StopRunning ();
}
