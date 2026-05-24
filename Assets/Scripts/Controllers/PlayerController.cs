using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 mouseLocalPos;
    
    private CellController cellController;

    void Start()
    {
        cellController = gameObject.GetComponent < CellController > ();
        if ( cellController == null )
        {
            Debug.LogError ( "PlayerController: It is an entity without Cell Controller" );
        }
    }

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
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint ( Input.mousePosition );
        mouseLocalPos = new Vector2 ( mouseWorldPos.x - transform.position.x,
                                      mouseWorldPos.y - transform.position.y );
        cellController.Move ( mouseLocalPos );
    }
}
