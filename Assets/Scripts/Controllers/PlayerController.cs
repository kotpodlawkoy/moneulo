using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 mouseLocalPos;
    
    private CellController cellController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cellController = gameObject.GetComponent < CellController > ();
        if ( cellController == null )
        {
            Debug.LogError ( "PlayerController: It is an entity without Cell Controller" );
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetKey ( KeyCode.W ) )
        {
            MovePlayer ();
        }
        if ( Input.GetKeyDown ( KeyCode.LeftShift ) )
        {
            cellController.StartRunning ();
        }
        if ( Input.GetKeyUp( KeyCode.LeftShift ) )
        {
            cellController.StopRunning ();
        }
        if ( Input.GetKey ( KeyCode.Space ) )
        {
            cellController.Heal ();
        }
        if ( Input.GetKeyDown ( KeyCode.B) && cellController is AdultCellController adult )
        {
            adult.Breed ();
        }
        if ( Input.GetKeyDown ( KeyCode.G ) && cellController is ChildCellController child )
        {
            child.Grow ();
        }
    }

    void MovePlayer ()
    {
        //Moving forward by pressing W
        //verticalInput = Input.GetAxis ( "Vertical" );
        //transform.Translate ( Vector2.up * verticalInput * cellController.speed * Time.deltaTime );
        
        //Calculating mouse position relatively to Player position
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint ( Input.mousePosition );
        mouseLocalPos = new Vector2 ( mouseWorldPos.x - transform.position.x,
                                      mouseWorldPos.y - transform.position.y );
        cellController.Move ( mouseLocalPos );
        //Rotate Player to appropriate angle
        //float rotateAngle = Vector2.SignedAngle ( Vector2.up, mouseLocalPos );
        //transform.rotation = Quaternion.Euler ( 0f, 0f, rotateAngle );
    }
}
