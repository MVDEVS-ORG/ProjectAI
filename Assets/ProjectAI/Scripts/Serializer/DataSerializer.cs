using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class DataSerializer
{
    public void SaveData<T>(string RelativePath, T Data)
    {
        string path = Application.persistentDataPath + RelativePath;
        try
        {
            if (File.Exists(path))
            {
                Debug.Log("Data Exists and is being overwritten");
                File.Delete(path);
            }
            else
            {
                Debug.Log("Writing new File");
            }

            using FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, JsonConvert.SerializeObject(Data,Formatting.Indented));

        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            Debug.Log("Storing Failed");
        }
    }

    public T LoadData<T>(string RelativePath)
    {
        string path = Application.persistentDataPath + RelativePath;
        if (!File.Exists(path))
        {
            Debug.LogError($"Cannot load file at {path}. File does not exist!");
            throw new FileNotFoundException($"{path} does not exist!");
        }

        T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        return data;
    }
}