using UnityEngine;
using UnityEngine.Events;

namespace BlueNoah.Download
{
	public static class DownloadingManager
	{
		private static GameObject go;
		private static DownloadingGetter downloadingGetter;
		private const string DOWNLOADING_MANAGER = "DownloadingManager";

		public static void Download (UnityAction downloadingComplete = null, UnityAction<float> downloadingProgress = null, UnityAction<string, string> downloadingError = null)
		{
			go = new GameObject (DOWNLOADING_MANAGER);
			downloadingGetter = go.AddComponent<DownloadingGetter> ();
			downloadingGetter.DownloadingComplete = () =>
			{
				if (downloadingComplete != null)
				{
					downloadingComplete ();
				}

				Object.Destroy (go);
			};
			downloadingGetter.DownloadingProgress = f =>
			{
				if (downloadingProgress != null)
				{
					downloadingProgress (f);
				}
			};
			downloadingGetter.DownloadingError = (name, error) =>
			{
				if (downloadingError != null)
				{
					downloadingError (name, error);
				}
			};
			downloadingGetter.Download ();
		}
	}
}
