using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

// TODO: Add encryption. Eventually add server side saving as well to make this an "all inclusive" utility.
namespace Covalent.Data {
	public class DataSerializer {

		public static void SerializeData<T>(T data, string path) {

			FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
			BinaryFormatter formatter = new BinaryFormatter();
			try {
				formatter.Serialize(fs, data);
				Debug.Log("Data written to " + path);
			} catch (SerializationException e) {
				Debug.LogError(e.Message);
			} finally {
				fs.Close();
			}
		}

		public static T DeserializeData<T>(string path) {

			T data = default(T);

			if (File.Exists(path)) {
				FileStream fs = new FileStream(path, FileMode.Open);
				try {
					BinaryFormatter formatter = new BinaryFormatter();
					data = (T)formatter.Deserialize(fs);
					Debug.Log("Data read from " + path);
				} catch (SerializationException e) {
					Debug.LogError(e.Message);
				} finally {
					fs.Close();
				}
			}

			return data;
		}
	}
}
