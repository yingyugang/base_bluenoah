using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BlueNoah.Download
{
	class DownloadingExecutor : MonoBehaviour
	{
		private Queue<DownloadingFileData> downloadingFileDataQueue;
		private const int MAX_DOWNLOADER = 5;
		private int downloadingFileDataCount;
		private float totalSize;
		private float finishedSize;
		private List<Downloader> downloaderList;
		private bool isNeedProgress;

		internal UnityAction<float> DownloadingProgress
		{
			get;
			set;
		}

		internal UnityAction DownloadingComplete
		{
			get;
			set;
		}

		internal UnityAction<string, string> DownloadingError
		{
			get;
			set;
		}

		private bool isDownloadingComplete;
		private bool isDownloadingStart;

		internal void StartDownload (Queue<DownloadingFileData> downloadingFileDataQueue, bool isNeedProgress = false, int totalSize = 0)
		{
			finishedSize = 0;
			isDownloadingComplete = false;
			this.downloadingFileDataQueue = downloadingFileDataQueue;
			this.isNeedProgress = isNeedProgress;
			this.totalSize = totalSize;
			downloadingFileDataCount = downloadingFileDataQueue.Count;
			downloaderList = new List<Downloader> ();
			int count = Mathf.Min (MAX_DOWNLOADER, downloadingFileDataQueue.Count);

			for (int i = 0; i < count; i++)
			{
				Downloader downloader = gameObject.AddComponent<Downloader> ();
				downloader.downloaderName = "Downloader" + i;
				downloader.downloaderComplete = DownloaderCompleteHandler;
				downloader.downloaderError = DownloaderErrorHandler;
				downloader.StartDownload (downloadingFileDataQueue.Dequeue ());
				downloaderList.Add (downloader);
			}

			isDownloadingStart = true;
		}

		private void DownloaderCompleteHandler (Downloader downloader, DownloadingFileData completedDownloadingFileData, AssetBundle assetBundle)
		{
			if (completedDownloadingFileData.FileType != DownloadingFileTypeEnum.CSV)
			{
				if (assetBundle != null)
				{
					assetBundle.Unload (true);
				}

				finishedSize += downloader.downloadingFileData.FileSize;
			}

			downloadingFileDataCount--;

			if (downloadingFileDataQueue.Count > 0)
			{
				DownloadingFileData downloadingFileData = downloadingFileDataQueue.Dequeue ();
				downloader.StartDownload (downloadingFileData);
			}
			else
			{
				downloaderList.Remove (downloader);
				downloader.downloaderComplete = null;
				downloader.downloaderError = null;
				Destroy (downloader);

				if (downloadingFileDataCount == 0)
				{
					isDownloadingComplete = true;
					isDownloadingStart = false;
					Invoke ("DownloadComplete", 1);
				}
			}
		}

		private void DownloadComplete ()
		{
			if (DownloadingComplete != null)
			{
				DownloadingComplete ();
			}
		}

		private void DownloaderErrorHandler (Downloader downloader, DownloadingFileData downloadingFileData, string error)
		{
			if (DownloadingError != null)
			{
				DownloadingError (downloadingFileData.FileName, error);
			}
		}

		private void Update ()
		{
			if (isDownloadingStart && isNeedProgress && !isDownloadingComplete)
			{
				GetProgress ();
			}
		}

		private void GetProgress ()
		{
			float currentSize = 0;

			for (int i = 0; i < downloaderList.Count; i++)
			{
				currentSize += downloaderList [i].CurrentSize;
			}

			DownloadingProgress ( (finishedSize + currentSize) / totalSize);
		}
	}
}
