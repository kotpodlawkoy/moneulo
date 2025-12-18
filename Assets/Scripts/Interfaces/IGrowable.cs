using UnityEngine;

public interface IGrowable
{
    GameObject Grow ();
    bool CanGrow { get; }
}
