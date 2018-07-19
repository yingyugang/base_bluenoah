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
            DownloadRemoteAssetConfig();
        }

        void InitDownload()
        {
            downloadingExecutor = gameObject.AddComponent<DownloadingExecutor>();
            //csvParser = gameObject.AddComponent<VersionCSVParser>();
            downloadingFileFilter = gameObject.AddComponent<DownloadingFileFilter>();
            downloadingFileDataQueueCreator = gameObject.AddComponent<DownloadingFileDataQueueCreator>();
            //assetCSVReader = gameObject.AddComponent<AssetCSVReader>();
            mLocalAssetConfig = JsonUtility.FromJson<AssetConfig>(FileManager.ReadString(DownloadConstant.LOCAL_VERSION_CONFIG_PATH));
            //if (DownloadConstant.CheckIfExistingVersionCSV())
            //{
            //    FileManager.DeleteFile(DownloadConstant.CLIENT_SERVER_VERSION_CSV);
            //    FileManager.DeleteFile(DownloadConstant.CLIENT_CLIENT_VERSION_CSV);
            //}
        }

        void DownloadRemoteAssetConfig()
        {
            StartCoroutine(DownloadRemoteConfig());
        }

        IEnumerator DownloadRemoteConfig()
        {
            UnityWebRequest www = UnityWebRequest.Get(DownloadConstant.REMOTE_VERSION_CONFIG_PATH + "?" + Random.Range(0, int.MaxValue));
            yield return www.Send();
            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                string downloadText = www.downloadHandler.text;
                mRemoteAssetConfig = JsonUtility.FromJson<AssetConfig>(downloadText);
                List<AssetConfigItem> items =  CheckDownloadList(mLocalAssetConfig,mRemoteAssetConfig);
            }
            else
            {
                Debug.LogError("Remove asset config is not existing.");
            }
        }

        List<AssetConfigItem> CheckDownloadList(AssetConfig localAssetConfig, AssetConfig removeAssetConfig)
        {
            List<AssetConfigItem> items = new List<AssetConfigItem>();
            Dictionary<string, AssetConfigItem> itemDic = new Dictionary<string, AssetConfigItem>();
            for (int i = 0; i < localAssetConfig.items.Count; i++)
            {
                itemDic.Add(localAssetConfig.items[i].assetName, localAssetConfig.items[i]);
            }

            for (int i = 0; i < removeAssetConfig.items.Count; i++)
            {
                if (itemDic.ContainsKey(removeAssetConfig.items[i].assetName))
                {
                    if(itemDic[removeAssetConfig.items[i].assetName].hashCode != removeAssetConfig.items[i].hashCode){
                        items.Add(removeAssetConfig.items[i]);
                    }
                }else{
                    items.Add(removeAssetConfig.items[i]);
                }
            }
            Debug.Log(items.Count);
            return items;
        }

        void StartDownload(List<AssetConfigItem> items){
            
        }

        bool CheckLocalConfigExsiting()
        {
            return FileManager.Exists(DownloadConstant.LOCAL_VERSION_CONFIG_PATH);
        }

        AssetConfig LoadRemoteConfig()
        {
            FileManager.ReadString(DownloadConstant.LOCAL_VERSION_CONFIG_PATH);
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

            AssetConfig assetConfig = JsonUtility.FromJson<AssetConfig>(DownloadConstant.LOCAL_VERSION_CONFIG_PATH);
            for (int i = 0; i < assetConfig.items.Count; i++)
            {

            }


            CsvContext mCsvContext = new CsvContext();
            IEnumerable<VersionCSVStructure> servers = mCsvContext.Read<VersionCSVStructure>(DownloadConstant.LOCAL_VERSION_CONFIG_PATH);
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
