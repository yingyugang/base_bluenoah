using UnityEngine;
using System.Collections.Generic;

namespace BlueNoah.Download
{
	class DownloadingFileDataQueueCreator : MonoBehaviour
	{
		internal int totalSize;

		internal Queue<DownloadingFileData> CreateConfigDownloadInfo ()
		{
			Queue<DownloadingFileData> downloadingFileDataQueue = new Queue<DownloadingFileData> ();    
            downloadingFileDataQueue.Enqueue (new DownloadingFileData (DownloadConstant.CONFIG_FILE, DownloadingFileTypeEnum.CSV, 0, 0, 0, null,""));
			return downloadingFileDataQueue;
		}

		internal Queue<DownloadingFileData> CreateForDownloadingAssets (List<VersionCSVStructure> filteredVersionCSVStructureList)
		{
			Queue<DownloadingFileData> downloadingFileDataQueue = new Queue<DownloadingFileData> ();

			for (int i = 0; i < filteredVersionCSVStructureList.Count; i++)
			{
				VersionCSVStructure versionCSVStructure = filteredVersionCSVStructureList [i];
				downloadingFileDataQueue.Enqueue (new DownloadingFileData (versionCSVStructure.FileName, DownloadingFileTypeEnum.Assets, versionCSVStructure.FileSize, versionCSVStructure.IsAssetBundle, versionCSVStructure.IsCSV, versionCSVStructure.HashCode,""));
				totalSize += versionCSVStructure.FileSize;
			}

			return downloadingFileDataQueue;
		}
	}


    public class VersionCSVStructure{
        public string FileName;
        public int FileSize;
        public int IsAssetBundle;
        public int IsCSV;
        public string HashCode;
    }
}
