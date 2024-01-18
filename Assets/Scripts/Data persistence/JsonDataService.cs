using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class JsonDataService : IDataService
{
	private const string Key = "ggdPhkeOoiv6YMiPWa34kIuOdDUL7NwQFg611DVdwN8=";
	private const string IV = "JZuM0HQsWSBVpRHTeRZMYQ==";

	public bool SaveData<T>(string relativePath, T data, bool encrypted)
	{
		string path = $"{Application.persistentDataPath}{relativePath}";

		try
		{
			if (File.Exists(path))
			{
				Debug.Log($"Data alredy exists in path: {path}. Deleting old file and writing a new one!");

				File.Delete(path);
			}
			else
				Debug.Log("Writing file for the first time!");
			
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
			Debug.LogError($"Unable to save data due to: {exception.Message} {exception.StackTrace}");

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

	public T LoadData<T>(string relativePath, bool encrypted)
	{
		string path = $"{Application.persistentDataPath}{relativePath}";

		if (!File.Exists(path))
		{
			Debug.LogError($"Cannot load file at {path}. File doesn't exist!");

			throw new FileNotFoundException($"{path} doesn't exist!");
		}

		try
		{
			T data;

			if (encrypted)
				data = ReadEncryptedData<T>(path);		
			else
				data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));		

			return data;
		}
		catch (Exception exception)
		{
			Debug.LogError($"Failed to load data due to: {exception.Message} {exception.StackTrace}");

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

		Debug.Log($"Decrypted result (if the following is not legible, probably wrond key or iv): {result}");
		return JsonConvert.DeserializeObject<T>(result);
	}
}
