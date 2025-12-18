using UnityEngine;

public interface IBreeder
{
    GameObject Breed ();
    bool CanBreed { get; }
}
