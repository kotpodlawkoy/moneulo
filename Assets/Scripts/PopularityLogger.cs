using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class PopulationLogger : MonoBehaviour
{
    public string herbivoreTag = "Herbivore";
    public string carnivoreTag = "Carnivore";

    public float sampleInterval = 1.0f;

    public string fileName = "population_log";

    private float _timer;
    private float _elapsed;
    private readonly StringBuilder _csv = new StringBuilder ();
    private string _filePath;

    void Start ()
    {
        _filePath = Path.Combine ( Application.dataPath, "..", fileName + ".csv" );
        _filePath = Path.GetFullPath ( _filePath );

        _csv.AppendLine ( "time,herbivores,carnivores" );

        Debug.Log ( "[PopulationLogger] Логирование начато. Файл: " + _filePath );
    }

    void Update ()
    {
        _timer += Time.deltaTime;
        _elapsed += Time.deltaTime;

        if ( _timer >= sampleInterval )
        {
            _timer = 0f;
            RecordSample ();
        }
    }

    void RecordSample ()
    {
        int herb = GameObject.FindGameObjectsWithTag ( herbivoreTag ).Length;
        int carn = GameObject.FindGameObjectsWithTag ( carnivoreTag ).Length;

        string line = string.Format (
            System.Globalization.CultureInfo.InvariantCulture,
            "{0:F1},{1},{2}", _elapsed, herb, carn );
        _csv.AppendLine ( line );
    }

    void SaveToFile ()
    {
        try
        {
            File.WriteAllText ( _filePath, _csv.ToString ());
            Debug.Log ( "[PopulationLogger] Данные сохранены: " + _filePath );
        }
        catch ( System.Exception e )
        {
            Debug.LogError ( "[PopulationLogger] Ошибка сохранения: " + e.Message );
        }
    }

    void OnApplicationQuit ()
    {
        SaveToFile ();
    }

    void OnDisable ()
    {
        SaveToFile ();
    }
}
