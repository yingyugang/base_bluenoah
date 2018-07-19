using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using BlueNoah.IO;

namespace BlueNoah.Download
{
    public class UnpackedAssetDownloader : SimpleSingleMonoBehaviour<UnpackedAssetDownloader>
    {
        private const string soundFileExtension = ".wav";


        Dictionary<string, AudioClip> downloadSounds = new Dictionary<string, AudioClip>();
        private Dictionary<string, Sprite> downloadImgs = new Dictionary<string, Sprite>();

        public AudioClip GetDownloadSound(string clipName)
        {
            return downloadSounds.ContainsKey(clipName) ? downloadSounds[clipName] : null;
        }

        public void LoadOrDownloadSounds(List<string> soundNames, UnityAction<Dictionary<string, AudioClip>> onComplete)
        {
            //UIManager.Instance.ShowLoadingShort();
            StartCoroutine(DownloadSounds(soundNames, onComplete));
        }

        //soundname just filename.e.g v1.wav.
        IEnumerator DownloadSounds(List<string> soundNames, UnityAction<Dictionary<string, AudioClip>> onComplete)
        {
            for (int i = 0; i < soundNames.Count; i++)
            {
                if (downloadSounds.ContainsKey(soundNames[i]))
                {
                    continue;
                }
                WWW www;
                string clientPath = DownloadConstant.CLIENT_SOUNDS_PATH + soundNames[i] + soundFileExtension;
                string serverPath = DownloadConstant.SERVER_SOUNDS_PATH + soundNames[i] + soundFileExtension;
                if (FileManager.Exists(clientPath))
                {
                    www = new WWW("file://" + clientPath);
                }
                else
                {
                    www = new WWW(serverPath);
                }
                yield return www;
                if (www.isDone)
                {
                    if (String.IsNullOrEmpty(www.error))
                    {
                        try
                        {
                            AudioClip clip = www.GetAudioClip();
                            if (clip != null)
                            {
                                downloadSounds.Add(soundNames[i], clip);
                                if (!FileManager.Exists(clientPath))
                                {
                                    FileManager.WriteAllBytes(clientPath, www.bytes);
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        finally
                        {
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            if (onComplete != null)
                onComplete(downloadSounds);
        }

        /****************************************************************
                                    Images
        *****************************************************************/

        //  InformationImg
        public void InformationLoadDownloadImages(List<string> usedDownloadImgPath, UnityAction onComplete = null)
        {
            LoadOrDownloadImages(DownloadConstant.INFORMATION_IMAGE_PATH, usedDownloadImgPath, onComplete);
        }

        // DownLoad Image
        public Sprite GetDownLoadImage(string path)
        {
            string _key = FilesPathToBase64(path);
            return downloadImgs.ContainsKey(_key) ? downloadImgs[_key] : null;
        }

        //imgnames must be full path.e.g http://10.10.10.10/xxx.png.
        private void LoadOrDownloadImages(string folderName, List<string> imgNames, UnityAction onComplete)
        {
            //  ディレクトリを作成
            string clientRootPath = DownloadConstant.CLIENT_IMAGES_PATH + folderName;
            if (!FileManager.DirectoryExisting(clientRootPath))
                Directory.CreateDirectory(clientRootPath);
            //  ディレクトリ内のファイルを取得
            string[] fileArray = FileManager.GetFiles(clientRootPath, "*", SearchOption.AllDirectories);
            List<string> filesList = new List<string>();
            filesList.AddRange(fileArray.Select(files => files.Replace(clientRootPath, "")));
            //  使用しないファイルを削除する
            List<string> imgNameBase64 = imgNames.Select(_img => FilesPathToBase64(_img)).ToList();
            foreach (string fileName in filesList)
                if (!imgNameBase64.Contains(fileName) && fileName.IndexOf("meta") < 0)
                    FileManager.DeleteFile(clientRootPath + fileName);   //  Don`t used Files.

            List<string> DLImgNameList = new List<string>();
            foreach (string imgname in imgNames)
            {
                string key = FilesPathToBase64(imgname);
                //  DL済みの場合
                if (filesList.Contains(key))
                {
                    downloadImgs[key] = GetSpriteFromPath(clientRootPath + key.Replace("/", "+"));
                }
                else
                {
                    DLImgNameList.Add(imgname);
                }
            }
            StartCoroutine(DownloadImages(clientRootPath, DLImgNameList, onComplete));
        }

        private IEnumerator DownloadImages(string clientRootPath, List<string> DLImgPathList, UnityAction onComplete)
        {
            foreach (string imgPath in DLImgPathList)
            {
                string _imgBasePath = FilesPathToBase64(imgPath);
                if (downloadImgs.ContainsKey(_imgBasePath))
                {
                    continue;
                }
                WWW www;
                string clientPath = clientRootPath + _imgBasePath;
                string serverPath = imgPath;
                if (FileManager.Exists(clientPath))
                {
                    www = new WWW("file://" + clientPath);
                }
                else
                {
                    www = new WWW(serverPath);
                }
                yield return www;
                if (www.isDone)
                {
                    if (String.IsNullOrEmpty(www.error))
                    {
                        if (www.texture != null)
                        {
                            Texture2D tex = www.texture;
                            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                            downloadImgs[_imgBasePath] = sprite;
                            if (!FileManager.Exists(clientPath))
                            {
                                FileManager.WriteAllBytes(clientPath, www.bytes);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            if (onComplete != null)
                onComplete();
        }

        //  File名をBase64にする
        private string FilesPathToBase64(string path)
        {
            //if (string.IsNullOrEmpty(path)) { return string.Empty; }
            //if (!path.Contains('.')) { return System.Utility.Base64Encode(path); }
            //string ext = path.Split('.').Last();
            //string fileName = path.Remove(path.Length - ext.Length - 1);
            //return (System.Utility.Base64Encode(fileName) + "." + ext);
            return "";
        }

        private Sprite GetSpriteFromPath(string filePath)
        {
            return GetSpriteFromByteArray(FileManager.ReadBytes(filePath));
        }

        private Sprite GetSpriteFromByteArray(byte[] bytes)
        {
            Texture2D texture = new Texture2D(2048, 2048);
            texture.LoadImage(bytes);
            texture.filterMode = FilterMode.Bilinear;
            texture.Compress(false);
            return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}
