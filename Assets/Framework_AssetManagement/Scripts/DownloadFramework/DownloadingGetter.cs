using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CSV;
using BlueNoah.IO;
using UnityEngine.Networking;
using System.Collections;

namespace BlueNoah.Download
{
    public class DownloadingGetter : MonoBehaviour
    {
        DownloadingExecutor downloadingExecutor;
        //private CSVParser csvParser;
        DownloadingFileFilter downloadingFileFilter;
        DownloadingFileDataQueueCreator downloadingFileDataQueueCreator;
        //private AssetCSVReader assetCSVReader;
        //private AssetBundleGetter assetBundleGetter;

        AssetConfig mRemoteAssetConfig;

        AssetConfig mLocalAssetConfig;

        internal UnityAction DownloadingComplete
        {
            get;
            set;
        }

        internal UnityAction<float> DownloadingProgress
        {
            get;
            set;
        }

        internal UnityAction<string, string> DownloadingError
        {
            get;
            set;
        }

        internal void Download()
        {
            InitDownload();
        }

        void InitDownload()
        {
            downloadingExecutor = gameObject.AddComponent<DownloadingExecutor>();
            //csvParser = gameObject.AddComponent<VersionCSVParser>();
            downloadingFileFilter = gameObject.AddComponent<DownloadingFileFilter>();
            downloadingFileDataQueueCreator = gameObject.AddComponent<DownloadingFileDataQueueCreator>();
            //assetCSVReader = gameObject.AddComponent<AssetCSVReader>();
            mLocalAssetConfig = JsonUtility.FromJson<AssetConfig>(FileManager.ReadString(DownloadConstant.DOWNLOAD_ASSET_CONFIG_PATH));
            //if (DownloadConstant.CheckIfExistingVersionCSV())
            //{
            //    FileManager.DeleteFile(DownloadConstant.CLIENT_SERVER_VERSION_CSV);
            //    FileManager.DeleteFile(DownloadConstant.CLIENT_CLIENT_VERSION_CSV);
            //}
        }

        void StartAssetDownload(List<AssetConfigItem> items){
            
        }

        void StartDownload(List<AssetConfigItem> items){
            
        }

        bool CheckLocalConfigExsiting()
        {
            return FileManager.Exists(DownloadConstant.DOWNLOAD_ASSET_CONFIG_PATH);
        }

        AssetConfig LoadRemoteConfig()
        {
            FileManager.ReadString(DownloadConstant.DOWNLOAD_ASSET_CONFIG_PATH);
            return null;
        }

        AssetConfig LoadLocalConfig()
        {
            return null;
        }

        void DownloadRemoteAssets()
        {
            if (!CheckLocalConfigExsiting())
            {

            }

            AssetConfig assetConfig = JsonUtility.FromJson<AssetConfig>(DownloadConstant.DOWNLOAD_ASSET_CONFIG_PATH);
            for (int i = 0; i < assetConfig.items.Count; i++)
            {

            }


            CsvContext mCsvContext = new CsvContext();
            IEnumerable<VersionCSVStructure> servers = mCsvContext.Read<VersionCSVStructure>(DownloadConstant.DOWNLOAD_ASSET_CONFIG_PATH);
            IEnumerable<VersionCSVStructure> server_resources = mCsvContext.Read<VersionCSVStructure>(DownloadConstant.CLIENT_SERVER_RESOURCE_VERSION_CSV);
            List<VersionCSVStructure> allServers = new List<VersionCSVStructure>();
            allServers.AddRange(servers);
            allServers.AddRange(server_resources);
            List<VersionCSVStructure> filteredVersionCSVStructureList = downloadingFileFilter.Filter(allServers);

            if (filteredVersionCSVStructureList.Count == 0)
            {
                if (DownloadingProgress != null)
                {
                    DownloadingProgress(1);
                }

                DownloadCompleteHandler();
                return;
            }

            downloadingExecutor.DownloadingProgress = progress =>
            {
                if (DownloadingProgress != null)
                {
                    DownloadingProgress(progress);
                }
            };
            downloadingExecutor.DownloadingError = (name, error) =>
            {
                if (DownloadingError != null)
                {
                    DownloadingError(name, error);
                }
            };
            downloadingExecutor.DownloadingComplete = DownloadCompleteHandler;
            Queue<DownloadingFileData> downloadingFileDataQueue = downloadingFileDataQueueCreator.CreateForDownloadingAssets(filteredVersionCSVStructureList);
            downloadingExecutor.StartDownload(downloadingFileDataQueue, true, downloadingFileDataQueueCreator.totalSize);
        }

        void DownloadCompleteHandler()
        {
            ParseCSV();
        }

        void FinishDownloading()
        {
            DisposeObject();
            //FileManager.CopyFile(DownloadConstant.LOCAL_VERSION_CONFIG, DownloadConstant.LOCAL_VERSION_CONFIG);
            FileManager.CopyFile(DownloadConstant.CLIENT_SERVER_RESOURCE_VERSION_CSV, DownloadConstant.CLIENT_CLIENT_RESOURCE_VERSION_CSV);
            if (DownloadingComplete != null)
            {
                DownloadingComplete();
            }
        }

        void ParseCSV()
        {
            //            CsvContext csvContext = new CsvContext();
            //            VersionCSV.versionCSV = csvContext.Read<VersionCSVStructure>(PathConstant.CLIENT_SERVER_VERSION_CSV);
            //            if (VersionCSV.versionCSV.FirstOrDefault(x => x.FileName == PathConstant.CSV) != null)
            //            {
            //                assetBundleGetter = gameObject.AddComponent<AssetBundleGetter>();
            //                assetBundleGetter.Get(AssetBundleName.csv.ToString(), loadedAssetBundle =>
            //                {
            //                    Destroy(assetBundleGetter);
            //                    assetCSVReader.Read(loadedAssetBundle.AssetBundle);
            //                });
            //            }
            FinishDownloading();
        }

        void DisposeObject()
        {
            downloadingExecutor.DownloadingComplete = null;
            downloadingExecutor.DownloadingProgress = null;
            downloadingExecutor.DownloadingError = null;
            Destroy(downloadingExecutor);
            //Object.Destroy(csvParser);
            Destroy(downloadingFileFilter);
            Destroy(downloadingFileDataQueueCreator);
            //Object.Destroy(assetCSVReader);
            //Object.Destroy(assetBundleGetter);
        }
    }
}
