using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnviromentSpawner : MonoBehaviour
{
    public GameObject [] wallPrefabs;
    public int wallAmount;

    public GameObject plantPrefab;
    public int startAmount;
    public int loopAmount;
    public float loopCoolDown;
    public float xZone, yZone;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine ( SpawnManyWalls ( xZone, yZone, wallAmount, OnWallsSpawned ) );
        
    }

    IEnumerator SpawnWall ( GameObject wallPrefub, float x, float y )
    {
        Vector3 pos = RandomPos ( x, y );
        float angle = Random.Range ( -90f, 90f );
        Quaternion qua = Quaternion.Euler ( 0f, 0f, angle );
        if ( Physics2D.OverlapBox ( new Vector2 ( pos.x, pos.y ),
                                    new Vector2 ( wallPrefub.gameObject.transform.localScale.x,
                                                  wallPrefub.gameObject.transform.localScale.y ),
                                    angle ) != null )
        {
            yield break;
        }

        Instantiate ( wallPrefub, pos, qua );
    }

    IEnumerator SpawnManyWalls ( float x, float y, int amount, System.Action onComplete )
    {
        for ( int i = 0; i < amount; i ++ )
        {
            GameObject randWall = wallPrefabs [ Random.Range ( 0, wallPrefabs.Length ) ];

            yield return StartCoroutine(SpawnWall(randWall, x, y));
        }
        onComplete?.Invoke ();
    }

    void SpawnPlantsLoop ()
    {
        SpawnManyPlants ( xZone, yZone, loopAmount );
    }

    void SpawnManyPlants ( float x, float y, int amount )
    {
        for ( int i = 0; i < amount; i ++ )
        {
            while ( !SpawnPlant ( x, y ) )
            {
                Debug.Log ( $"x = {x}, y = {y}" );
            }
        }
    }

    bool SpawnPlant ( float x, float y )
    {
        Vector3 pos = RandomPos ( x, y );
        float colSize = plantPrefab.GetComponent < CircleCollider2D > ().radius;
        Collider2D col = Physics2D.OverlapCircle ( new Vector2 ( pos.x, pos.y ), colSize );
        if ( col != null )
        {
            if ( col.CompareTag ( "Wall" ) ) return false;
        }
        Instantiate ( plantPrefab, pos, plantPrefab.transform.rotation );
        return true;
    }

    Vector3 RandomPos ( float x, float y )
    {
        float randX = Random.Range ( -x, x );
        float randY = Random.Range ( -y, y );
        return new Vector3 ( randX, randY, 0f );
    }

    void OnWallsSpawned ()
    {
        SpawnManyPlants ( xZone, yZone, startAmount );
        InvokeRepeating ( "SpawnPlantsLoop", loopCoolDown, loopCoolDown );
    }
}
