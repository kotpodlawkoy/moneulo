using UnityEngine;

public interface IEater
{
    void Eat ( GameObject food );
    string FoodTag { get; }
}
