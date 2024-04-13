using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace DataPersistance
{
	public class JsonDataService : IDataService
	{
		private const string Key = "ggdPhkeOoiv6YMiPWa34kIuOdDUL7NwQFg611DVdwN8=";
		private const string IV = "JZuM0HQsWSBVpRHTeRZMYQ==";

		public bool SaveData<T>(string relativePath, T data, bool encrypted)
		{
			string path = $"{Application.persistentDataPath}/{relativePath}";

			try
			{
				if (File.Exists(path))
				{
#if UNITY_EDITOR
					Debug.Log($"Data alredy exists in path: {path}. <color=green>Deleting old file and writing a new one!</color>");
#endif
					File.Delete(path);
				}
				else
				{
#if UNITY_EDITOR
					Debug.Log("<color=green>Writing file for the first time!</color>");
#endif
				}

				using FileStream stream = File.Create(path);

				if (encrypted)
					WriteEncryptedData(data, stream);
				else
				{
					stream.Close();

					File.WriteAllText(path, JsonConvert.SerializeObject(data));
				}

				return true;
			}
			catch (Exception exception)
			{
#if UNITY_EDITOR
				Debug.LogError($"Unable to save data due to: {exception.Message} {exception.StackTrace}");
#endif
				return false;
			}
		}

		private void WriteEncryptedData<T>(T data, FileStream stream)
		{
			using Aes aesProvider = Aes.Create();

			aesProvider.Key = Convert.FromBase64String(Key);
			aesProvider.IV = Convert.FromBase64String(IV);

			using ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor();
			using CryptoStream cryptoStream = new(stream, cryptoTransform, CryptoStreamMode.Write);

			cryptoStream.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(data)));
		}

		public bool LoadData<T>(out T data, string relativePath, bool encrypted)
		{
			string path = $"{Application.persistentDataPath}/{relativePath}";
			
			if (!File.Exists(path))
			{
#if UNITY_EDITOR
				Debug.LogError($"Cannot load file at {path}. File doesn't exist!");
#endif
				data = default(T);	

				return false;

				throw new FileNotFoundException($"{path} doesn't exist!");
			}

			try
			{
				if (encrypted)
					data = ReadEncryptedData<T>(path);
				else
					data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));

				return true;
			}
			catch (Exception exception)
			{
#if UNITY_EDITOR
				Debug.LogError($"Failed to load data due to: {exception.Message} {exception.StackTrace}");
#endif
				data = default(T);

				return false;

				throw exception;
			}
		}

		private T ReadEncryptedData<T>(string path)
		{
			byte[] fileBytes = File.ReadAllBytes(path);

			using Aes aesProvider = Aes.Create();

			aesProvider.Key = Convert.FromBase64String(Key);
			aesProvider.IV = Convert.FromBase64String(IV);

			using ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(aesProvider.Key, aesProvider.IV);

			using MemoryStream decryptionStream = new(fileBytes);
			using CryptoStream cryptoStream = new(decryptionStream, cryptoTransform, CryptoStreamMode.Read);

			using StreamReader reader = new(cryptoStream);

			string result = reader.ReadToEnd();
#if UNITY_EDITOR
			Debug.Log($"<color=green>Decrypted result</color> (if the following is not legible, probably wrond key or iv): {result}");
#endif
			return JsonConvert.DeserializeObject<T>(result);
		}
	}
}
