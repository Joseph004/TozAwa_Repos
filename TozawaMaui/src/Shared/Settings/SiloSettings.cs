namespace Shared.Settings
{
    public class SiloSettings
    {
		#region Constants

		public const int FILE_PROCESSING_INTERVAL = 1;
		public const string DEFAULT_LARGE_STATE_STORAGE_CONNECTION_STRING = "UseDevelopmentStorage=true";
		public const string DEFAULT_LARGE_STATE_STORAGE_NAME = "pricefilemanager-grainstate";
		public const string DEFAULT_STATE_STORAGE_CONNECTION_STRING = "UseDevelopmentStorage=true";
		public const string DEFAULT_STATE_STORAGE_NAME = "PriceFileManagerGrainState";

		#endregion

		#region Properties (public)

		public int FileProcessingInterval { get; set; } = FILE_PROCESSING_INTERVAL;
		public string LargeStateStorageConnectionString { get; set; } = DEFAULT_LARGE_STATE_STORAGE_CONNECTION_STRING;
		public string LargeStateStorageName { get; set; } = DEFAULT_LARGE_STATE_STORAGE_NAME;
		public string StateStorageConnectionString { get; set; } = DEFAULT_STATE_STORAGE_CONNECTION_STRING;
		public string StateStorageName { get; set; } = DEFAULT_STATE_STORAGE_NAME;

		#endregion
	}
}