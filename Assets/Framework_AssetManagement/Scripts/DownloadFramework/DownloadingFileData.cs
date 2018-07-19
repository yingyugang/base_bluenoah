namespace BlueNoah.Download
{
	class DownloadingFileData
	{
		internal string FileName
		{
			get;
			set;
		}

		internal DownloadingFileTypeEnum FileType
		{
			get;
			set;
		}

		internal int FileSize
		{
			get;
			set;
		}

		internal int IsAssetBundle
		{
			get;
			set;
		}

		private int IsCSV
		{
			get;
			set;
		}

		private string HashCode
		{
			get;
			set;
		}

        private string StorePath{
            get;
            set;
        }

		public DownloadingFileData (string fileName, DownloadingFileTypeEnum fileType, int fileSize, int isAssetBundle, int isCSV, string hashCode,string storePath)
		{
			FileName = fileName;
			FileType = fileType;
			FileSize = fileSize;
			IsAssetBundle = isAssetBundle;
			IsCSV = isCSV;
			HashCode = hashCode;
            StorePath = storePath;
		}
	}
}
