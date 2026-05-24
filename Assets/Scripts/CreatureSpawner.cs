using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpeciesEntry
    {
        public GameObject prefab;
        public int targetPopulation = 20;
        public string tag;
        public int initialSpawn = 10;
    }

    [Header("Species")]
    public SpeciesEntry[] species;

    [Header("Spawn area")]
    public float xZone = 498f;
    public float yZone = 498f;

    [Header("Spawn safety")]
    public float spawnCheckRadius = 0.5f;
    public int maxSpawnAttempts = 30;
    public LayerMask blockingLayers;

    [Header("Population control")]
    public float checkInterval = 1f;

    private float _timer;
    private bool _ready = false;

    void OnEnable()
    {
        EnviromentSpawner.WallsSpawned += OnWallsReady;
    }

    void OnDisable()
    {
        EnviromentSpawner.WallsSpawned -= OnWallsReady;
    }
    void OnWallsReady()
    {
        StartCoroutine(SpawnInitialPopulation());
    }

    System.Collections.IEnumerator SpawnInitialPopulation()
    {
        foreach (var s in species)
        {
            for (int i = 0; i < s.initialSpawn; i++)
            {
                TrySpawnOne(s);
                yield return new WaitForSeconds(0.1f);
            }
        }
        _ready = true;
    }

    void Update()
    {
        if (!_ready) return;

        _timer += Time.deltaTime;
        if (_timer < checkInterval) return;
        _timer = 0f;

        foreach (var s in species)
        {
            int currentCount = GameObject.FindGameObjectsWithTag(s.tag).Length;
            int needed = s.targetPopulation - currentCount;
            for (int i = 0; i < needed; i++)
            {
                TrySpawnOne(s);
            }
        }
    }

    bool TrySpawnOne(SpeciesEntry s)
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Vector2 pos = new Vector2(Random.Range(-xZone, xZone), Random.Range(-yZone, yZone));
            if (Physics2D.OverlapCircle(pos, spawnCheckRadius, blockingLayers) == null)
            {
                Instantiate(s.prefab, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
                return true;
            }
        }
        return false;
    }
}
