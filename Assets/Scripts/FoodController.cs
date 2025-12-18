using UnityEngine;

public class FoodController : MonoBehaviour
{
    public float _currentFoodAmount;
    public float maxFoodAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FoodAmount = maxFoodAmount;
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
