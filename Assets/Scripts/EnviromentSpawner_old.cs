using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnviromentSpawner_old : MonoBehaviour
{
    public GameObject [] wallPrefabs;
    public int wallAmount;
    public GameObject mainCamera;

    public GameObject plantPrefab;
    public int startAmount;
    public int loopAmount;
    public float loopCoolDown;
    public float xZone, yZone;

    private struct DebugBoxData
    {
        public Vector2 center;
        public Vector2 size;
        public float angle;
        public Color color;
        public float duration;
        public float timestamp;
    }
    
    private List<DebugBoxData> debugBoxes = new List<DebugBoxData>();

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
        //Vector3 colSize = wallPrefub.GetComponent < BoxCollider2D > ().size;
        if ( Physics2D.OverlapBox ( new Vector2 ( pos.x, pos.y ),
                                    new Vector2 ( wallPrefub.gameObject.transform.localScale.x,
                                                  wallPrefub.gameObject.transform.localScale.y ),
                                    angle ) != null )
        {
            yield break;
            //return false;
        }
        //AddDebugBox(pos, new Vector2 ( wallPrefub.gameObject.transform.localScale.x,
        //                               wallPrefub.gameObject.transform.localScale.y ), angle, Color.red, 300f);
        //DrawDebugBox( new Vector2 ( pos.x, pos.y ),
        //              new Vector2 ( wallPrefub.gameObject.transform.localScale.x,
        //                            wallPrefub.gameObject.transform.localScale.y ) / 2,
        //              angle );
        //DrawOverlapBox ( new Vector2 ( pos.x, pos.y ),
        //                 new Vector2 ( wallPrefub.gameObject.transform.localScale.x,
        //                               wallPrefub.gameObject.transform.localScale.y ) / 2,
        //                 angle );
        //MoveCameraTo ( pos.x, pos.y );
        //yield return WaitForInput (KeyCode.N);

        Instantiate ( wallPrefub, pos, qua );
        //Debug.Log ( $"wall = {pos}, localScale = {wallPrefub.gameObject.transform.localScale}, time = {Time.time}, angle = {angle}" );
        //return true;
    }

    IEnumerator SpawnManyWalls ( float x, float y, int amount, System.Action onComplete )
    {
        for ( int i = 0; i < amount; i ++ )
        {
            GameObject randWall = wallPrefabs [ Random.Range ( 0, wallPrefabs.Length ) ];
            //while ( !SpawnWall ( randWall, x, y ) )
            //{
            //    //Debug.Log ( $"wall = {randWall.name}, x = {x}, y = {y}" );
            //}
            yield return StartCoroutine(SpawnWall(randWall, x, y));
            //yield return null;
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

    void DrawOverlapBox ( Vector2 center, Vector2 size, float angle )
    {
        // Переводим угол в радианы
        float angleRad = angle * Mathf.Deg2Rad;
        
        // Вычисляем половины размеров
        float halfWidth = size.x / 2f;
        float halfHeight = size.y / 2f;
        
        // Углы прямоугольника (без поворота)
        Vector3[] corners = new Vector3[4]
        {
            new Vector3(-halfWidth, -halfHeight, -1.0f), // левый нижний
            new Vector3(-halfWidth,  halfHeight, -1.0f), // левый верхний
            new Vector3( halfWidth,  halfHeight, -1.0f), // правый верхний
            new Vector3( halfWidth, -halfHeight, -1.0f)  // правый нижний
        };
        
        // Поворачиваем и смещаем углы
        for (int i = 0; i < corners.Length; i++)
        {
            // Поворот
            float x = corners[i].x * Mathf.Cos(angleRad) - corners[i].y * Mathf.Sin(angleRad);
            float y = corners[i].x * Mathf.Sin(angleRad) + corners[i].y * Mathf.Cos(angleRad);
            
            // Смещение к центру
            corners[i] = new Vector3(x + center.x, y + center.y, -1.0f );
        }
        
        // Рисуем линии
        Debug.DrawLine(corners[0], corners[1], Color.red);    // левая
        Debug.DrawLine(corners[1], corners[2], Color.green);  // верхняя
        Debug.DrawLine(corners[2], corners[3], Color.blue);   // правая
        Debug.DrawLine(corners[3], corners[0], Color.yellow); // нижняя
        
        // Диагонали для наглядности
        Debug.DrawLine(corners[0], corners[2], Color.white * 0.5f);
        Debug.DrawLine(corners[1], corners[3], Color.white * 0.5f);
    }

    IEnumerator WaitForInput ( KeyCode key )
    {
        Debug.Log($"Нажми {key} чтобы продолжить...");
        
        while (!Input.GetKeyDown(key))
        {
            yield return null;
        }
        
        Debug.Log($"Клавиша {key} нажата!");
    }

    void MoveCameraTo ( float x, float y )
    {
        mainCamera.transform.position = new Vector3 ( x, y, -20f );
    }
    void DrawDebugBox(Vector2 center, Vector2 size, float angle)
    {
        #if UNITY_EDITOR
        // Рисуем красный куб там, где проверяем
        Matrix4x4 matrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);
        Gizmos.matrix = matrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, size.y, 0.1f));
        Gizmos.matrix = Matrix4x4.identity;
        #endif
        }
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        // Удаляем старые боксы
        debugBoxes.RemoveAll(b => Time.time - b.timestamp > b.duration);
        
        // Рисуем все активные боксы
        foreach (var box in debugBoxes)
        {
            DrawGizmoBox(box.center, box.size, box.angle, box.color);
        }
    }
    void AddDebugBox(Vector2 center, Vector2 size, float angle, Color color, float duration)
    {
        debugBoxes.Add(new DebugBoxData
        {
            center = center,
            size = size,
            angle = angle,
            color = color,
            duration = duration,
            timestamp = Time.time
        });
    }
    void DrawGizmoBox(Vector2 center, Vector2 size, float angle, Color color)
    {
        Gizmos.color = color;
        
        // Сохраняем текущую матрицу
        Matrix4x4 originalMatrix = Gizmos.matrix;
        
        // Устанавливаем матрицу трансформации
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(
            new Vector3(center.x, center.y, 0),
            Quaternion.Euler(0, 0, angle),
            Vector3.one
        );
        
        Gizmos.matrix = rotationMatrix;
        
        // Рисуем куб
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, size.y, 0.1f));
        
        // Восстанавливаем матрицу
        Gizmos.matrix = originalMatrix;
    }
}
