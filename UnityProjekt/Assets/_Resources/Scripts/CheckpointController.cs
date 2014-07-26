using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

using Object = System.Object;

public class CheckpointController : MonoBehaviour {

	public static void SaveState(string filename, Object obj) {
                try {
                        Stream fileStream  = File.Open(filename, FileMode.Create);
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(fileStream, obj);
                        fileStream.Close();
                } catch(Exception e) {
                        Debug.LogWarning("Save.SaveFile(): Failed to serialize object to a file " + filename + " (Reason: " + e.ToString() + ")");
                }
        }

        public static Object LoadState(string filename) {
                try {
                        Stream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read);
                        BinaryFormatter formatter = new BinaryFormatter();
                        Object obj = formatter.Deserialize(fileStream);
                        fileStream.Close();
                        return obj;
                } catch(Exception e) {
                        Debug.LogWarning("SaveLoad.LoadFile(): Failed to deserialize a file " + filename + " (Reason: " + e.ToString() + ")");
                        return null;
                }       
        }
}
