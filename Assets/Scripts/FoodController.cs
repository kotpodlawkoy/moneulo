using UnityEngine;

public class FoodController : MonoBehaviour
{
    public float _currentFoodAmount;
    public float maxFoodAmount;
    public float TimeMeatToRot;

    void Start ()
    {
        FoodAmount = maxFoodAmount;
        if ( gameObject.tag == "Meat" )
        {

            Invoke ( nameof ( Die ), TimeMeatToRot );
        }
    }

    public void GetEaten ( float eatAmount )
    {
        FoodAmount -= eatAmount;
        RescaleFood ();
    }

    void RescaleFood ()
    {
        transform.localScale = Vector3.one * ( FoodAmount + maxFoodAmount ) /
                               ( maxFoodAmount + maxFoodAmount );
    }

    void Die ()
    {
        Destroy ( gameObject );
    }

    public float FoodAmount
    {
        get
        {
            return _currentFoodAmount;
        }
        set
        {
            if ( value <= 0f )
            {
                Die ();
            }
            else
            {
                _currentFoodAmount = ( value > maxFoodAmount ) ? maxFoodAmount : value;
            }
        }
    }
}
