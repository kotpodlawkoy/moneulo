//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine.Networking;
//
//public class AgentVisibilityController : BaseVisionController
//{
//    [Header("NocoDB Settings")]
//    public string nocoDBUrl = "https://your-nocodb.com/api/v2/tables";
//    public string apiToken = "your-token";
//    public string perceptionTableId = "your-perception-table-id";
//    
//    [Header("Settings")]
//    public float logInterval = 1f;
//    public int batchSize = 5;
//    
//    private List<PerceptionRecord> batch = new List<PerceptionRecord>();
//    
//    [System.Serializable]
//    private class PerceptionRecord
//    {
//        public string Title;
//        public string percepted_object_type;
//        public float percepted_relative_position_x;
//        public float percepted_relative_position_y;
//    }
//    
//    void Start()
//    {
//        StartCoroutine(LogRoutine());
//    }
//    
//    IEnumerator LogRoutine()
//    {
//        while (true)
//        {
//            yield return new WaitForSeconds(logInterval);
//            RecordPerceptions();
//        }
//    }
//    
//    void RecordPerceptions()
//    {
//        foreach (GameObject target in visibleTargets)
//        {
//            if (target == null || target == gameObject) continue; // Skip self
//            
//            Vector2 relativePosition = target.transform.position - transform.position;
//            
//            var record = new PerceptionRecord
//            {
//                Title = "Hello World!",
//                percepted_object_type = target.tag,
//                percepted_relative_position_x = relativePosition.x,
//                percepted_relative_position_y = relativePosition.y
//            };
//            
//            batch.Add(record);
//        }
//        
//        // Upload when batch reaches size
//        if (batch.Count >= batchSize)
//        {
//            StartCoroutine(UploadBatch());
//        }
//    }
//    
//    IEnumerator UploadBatch()
//    {
//        var recordsToUpload = new List<PerceptionRecord>(batch);
//        batch.Clear();
//        
//        string url = $"{nocoDBUrl}/{perceptionTableId}/records";
//        
//        var requestData = new
//        {
//            records = recordsToUpload.Select(r => new { fields = r }).ToList()
//        };
//
//        Debug.Log ( $"requestData = {requestData}" );
//        
//        string jsonData = JsonUtility.ToJson(requestData);
//        Debug.Log ( $"jsonData = {jsonData}" );
//        
//        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
//        {
//            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
//            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
//            request.downloadHandler = new DownloadHandlerBuffer();
//            request.SetRequestHeader("Content-Type", "application/json");
//            request.SetRequestHeader("xc-token", apiToken);
//            
//            yield return request.SendWebRequest();
//            
//            if (request.result == UnityWebRequest.Result.Success)
//            {
//                Debug.Log($"Uploaded {recordsToUpload.Count} perceptions");
//            }
//            else
//            {
//                Debug.LogError($"Upload failed: {request.error}");
//            }
//        }
//    }
//
//    void OnDestroy()
//    {
//        if (batch.Count > 0)
//        {
//            StartCoroutine(UploadBatch());
//        }
//    }
//}
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class AgentVisibilityController : BaseVisionController
{
    [Header("NocoDB Settings")]
    public string nocoDBUrl = "https://your-nocodb.com/api/v2/tables";
    public string apiToken = "your-token";
    public string perceptionTableId = "your-perception-table-id";
    
    [Header("Settings")]
    public float logInterval = 1f;
    public int batchSize = 5;
    
    private List<PerceptionRecord> batch = new List<PerceptionRecord>();
    
    [System.Serializable]
    private class PerceptionRecord
    {
        public string Title;
        public string percepted_object_type;
        public float percepted_relative_position_x;
        public float percepted_relative_position_y;
    }
    
    // Класс-обертка для fields
    [System.Serializable]
    private class RecordWithFields
    {
        public PerceptionRecord fields;
        
        public RecordWithFields(PerceptionRecord record)
        {
            fields = record;
        }
    }
    
    void Start()
    {
        StartCoroutine(LogRoutine());
    }
    
    IEnumerator LogRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(logInterval);
            RecordPerceptions();
        }
    }
    
    void RecordPerceptions()
    {
        foreach (GameObject target in visibleTargets)
        {
            if (target == null || target == gameObject) continue; // Skip self
            
            Vector2 relativePosition = target.transform.position - transform.position;
            
            var record = new PerceptionRecord
            {
                Title = "Hello World!",
                percepted_object_type = target.tag,
                percepted_relative_position_x = relativePosition.x,
                percepted_relative_position_y = relativePosition.y
            };
            
            batch.Add(record);
        }
        
        // Upload when batch reaches size
        if (batch.Count >= batchSize)
        {
            StartCoroutine(UploadBatch());
        }
    }
    
    IEnumerator UploadBatch()
    {
        var recordsToUpload = new List<PerceptionRecord>(batch);
        batch.Clear();
        
        string url = $"{nocoDBUrl}/{perceptionTableId}/records";
        
        // Конвертируем записи в формат с fields
        List<RecordWithFields> wrappedRecords = recordsToUpload
            .Select(r => new RecordWithFields(r))
            .ToList();
        
        // Создаем JSON как массив объектов с fields
        string jsonData = JsonHelper.ToJsonArray(wrappedRecords);
        
        Debug.Log($"jsonData = {jsonData}");
        
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("xc-token", apiToken);
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"✅ Uploaded {recordsToUpload.Count} perceptions");
                Debug.Log($"Response: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"❌ Upload failed: {request.error}");
                Debug.LogError($"Response code: {request.responseCode}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
                
                // Возвращаем записи в batch при ошибке
                batch.AddRange(recordsToUpload);
            }
        }
    }
    
    void OnDestroy()
    {
        if (batch.Count > 0)
        {
            StartCoroutine(UploadBatch());
        }
    }
}

// Хелпер для сериализации массива в JSON
public static class JsonHelper
{
    public static string ToJsonArray<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
            return "[]";
        
        string[] jsonItems = new string[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            jsonItems[i] = JsonUtility.ToJson(list[i]);
        }
        
        return "[" + string.Join(",", jsonItems) + "]";
    }
}
