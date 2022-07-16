using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SerialisationManager
{
    private static string PATH = Application.persistentDataPath + "UserSettings/settings.prefs";
    public static void SaveSettings(GameSettings _settings)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = PATH;
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, _settings);
        stream.Close();
    }

    public static GameSettings LoadSettings()
    {
        // TODO: Make this a try/catch
        if(File.Exists(PATH))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(PATH, FileMode.Open);

            GameSettings data = formatter.Deserialize(stream) as GameSettings;
            stream.Close();

            return data;
        }
        else 
        {
            Debug.Log("Error: Serialisation path not found!");
            return null;
        }
    }
}
