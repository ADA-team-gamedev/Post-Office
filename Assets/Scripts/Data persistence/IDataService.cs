namespace DataPersistance
{
	public interface IDataService
	{
		public bool SaveData<T>(string relativePath, T data, bool encrypted);

		public bool TryLoadData<T>(out T data, string relativePath, bool encrypted);
	}
}