using UnityEngine;

public interface ILiving
{
    bool CanLive { get; }

    void Live ();
}
