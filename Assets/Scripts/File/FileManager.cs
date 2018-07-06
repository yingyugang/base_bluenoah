/*
 * 作者:曾飞			创建⽇日期:2014-11-20
 * 描述: 文件读取、写入管理类.
 * */
using System;
using System.IO;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using CSV;

namespace BlueNoah.IO
{
    public class FileManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileManager"/> class.
        /// </summary>
        public FileManager()
        {
        }

        #region 判断文件是否存在.

        /// <summary>
        /// 判断文件是否存在.
        /// </summary>
        /// <param name="path">Path.</param>
        public static bool Exists(string path)
        {
            return File.Exists(path);
        }

        #endregion

        #region 判断文件夹是否存在.

        /// <summary>
        /// 判断文件夹是否存在.
        /// </summary>
        /// <returns><c>true</c>, if exists was directoryed, <c>false</c> otherwise.</returns>
        /// <param name="path">Path.</param>
        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        #endregion

        #region 生成路径.

        /// <summary>
        /// Assets路径生成.
        /// </summary>
        /// <returns>The assets path format.</returns>
        /// <param name="relativePath">Relative path.</param>
        public static string AssetsPathFormat(string relativePath)
        {
            string path = "";
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.Android)
            {
                path = (Application.persistentDataPath + "/" + relativePath).Replace(" ", "%20");
            }
            else
            {
                path = Application.dataPath + "/" + relativePath;
            }

            return path;
        }


        /// <summary>
        /// Resources路径生成.
        /// </summary>
        /// <returns>The assets path format.</returns>
        /// <param name="relativePath">Relative path.</param>
        public static string ResourcesPathFormat(string relativePath)
        {
            string path = "";
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.WebGLPlayer)
            {
                path = (Application.persistentDataPath + "/Resources/" + relativePath).Replace(" ", "%20");
            }
            else
            {
                path = Application.dataPath + "/Resources/" + relativePath;
            }

            return path;
        }

        /// <summary>
        /// StreamingAssets路径生成.
        /// </summary>
        /// <returns>The assets path format.</returns>
        /// <param name="relativePath">Relative path.</param>
        public static string StreamingPathFormat(string relativePath)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return (Application.streamingAssetsPath + "/" + relativePath).Replace(" ", "%20");
            }
            return (Application.streamingAssetsPath + "/" + relativePath).Replace(" ", "%20");
        }

        public static string GetAssetPath(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            string path = UnityEditor.AssetDatabase.GetAssetPath(obj);
            path = path.Replace("Assets", "");
            return Application.dataPath + path;
#else
			return "";
#endif
        }

        #endregion

        #region 把数据以二进制的形式写入到指定文件中.

        /// <summary>
        /// 把数据以二进制的形式写入到指定文件中.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <param name="bytes">Bytes.</param>
        public static void WriteAllBytes(string filePath, byte[] bytes)
        {
            //TODO modify by xiekun 20140617
            //			if (bytes == null || bytes.Length == 0) 
            //			{
            //				Debug.Log("save file error, bytes is null.");
            //				return;
            //			}
#if UNITY_IPHONE
			UnityEngine.iOS.Device.SetNoBackupFlag (filePath);
#endif
            try
            {
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

            }
            catch (System.Exception e)
            {
                //to do
                Debug.Log("IO error." + e.Message);
            }
            try
            {
                File.WriteAllBytes(filePath, bytes);
            }
            catch (System.Exception e)
            {
                Debug.Log("IO error ." + e.Message);
            }
        }

        #endregion

        #region 在文件内容末尾继续写入.

        /// <summary>
        /// Appends all text.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <param name="str">String.</param>
        public static void AppendAllText(string filePath, string str)
        {
#if UNITY_IPHONE
			UnityEngine.iOS.Device.SetNoBackupFlag (filePath);
#endif
            try
            {
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

            }
            catch (System.Exception e)
            {
                //to do
                Debug.Log("IO error." + e.Message);
            }


            try
            {
                File.AppendAllText(filePath, str);
            }
            catch (System.Exception e)
            {
                Debug.Log("IO error ." + e.Message);
            }
        }

        #endregion

        #region 把字符串写入到文件中.

        /// <summary>
        /// 把字符串写入到文件中.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <param name="contents">Contents.</param>
        public static void WriteString(string filePath, string contents)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(contents);

            FileManager.WriteAllBytes(filePath, bytes);
        }

        #endregion

        #region 读取本地文件返回字符串.

        /// <summary>
        /// 读取本地文件返回字符串.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="filePath">File path.</param>
        public static string ReadString(string filePath)
        {
            if (!FileManager.Exists(filePath))
            {
                return "";
            }
            string contents = "";
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(filePath);

                contents = reader.ReadToEnd();
            }
            catch (System.Exception e)
            {
                Debug.Log("IO Read error ." + e.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                    reader = null;
                }
            }
            return contents;
        }

        #endregion

        #region 读取本地文件返回字符串集合.

        /// <summary>
        /// 读取本地文件返回字符串集合.
        /// </summary>
        /// <returns>The lines.</returns>
        /// <param name="filePath">File path.</param>
        public static List<string> ReadLines(string filePath)
        {
            List<string> contents = new List<string>();
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                while (!reader.EndOfStream)
                {
                    contents.Add(reader.ReadLine());
                }

            }
            catch (System.Exception e)
            {
                Debug.Log("IO Read error ." + e.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                    reader = null;
                }
            }
            return contents;
        }

        #endregion

        #region 读取本地文件返回二进制流.

        /// <summary>
        /// 读取本地文件返回二进制流.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="filePath">File path.</param>
        public static byte[] ReadBytes(string filePath)
        {
            StreamReader reader = null;
            byte[] buffer = null;
            try
            {
                reader = new StreamReader(filePath);
                Stream stream = reader.BaseStream;
                if (stream.CanRead)
                {
                    buffer = new byte[(int)stream.Length];
                    stream.Read(buffer, 0, (int)stream.Length);
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                }
                reader.Close();
                reader.Dispose();
                reader = null;

            }
            catch (System.Exception e)
            {
                Debug.Log("IO Read error ." + e.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                    reader = null;
                }
            }
            return buffer;
        }

        #endregion

        #region 读取本地文件返回Texture2D.

        /// <summary>
        /// 读取本地文件返回Texture2D.
        /// </summary>
        /// <returns>The texture2d.</returns>
        /// <param name="filePath">File path.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public static Texture2D ReadTexture2D(string filePath, int width, int height)
        {
            StreamReader reader = null;
            byte[] buffer = null;
            try
            {
                reader = new StreamReader(filePath);
                Stream stream = reader.BaseStream;
                if (stream.CanRead)
                {
                    buffer = new byte[(int)stream.Length];
                    stream.Read(buffer, 0, (int)stream.Length);
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                }
                reader.Close();
                reader.Dispose();
                reader = null;
            }
            catch (System.Exception e)
            {
                Debug.Log("IO Read error .path:" + filePath + ". message:" + e.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                    reader = null;
                }
            }
            if (buffer == null || buffer.Length == 0)
            {
                return null;
            }
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture2D.LoadImage(buffer);
            texture2D.anisoLevel = 2;
            texture2D.Compress(true);
            return texture2D;
        }

        #endregion

        #region 获取文件的hash值.

        /// <summary>
        /// 获取文件的hash值.
        /// </summary>
        /// <returns>
        /// The file's hash.
        /// </returns>
        /// <param name='filePath'>
        /// File path.
        /// </param>
        public static string GetFileHash(string filePath)
        {
            string hashcode = "";
            FileInfo info = new FileInfo(filePath);
            if (info.Exists)
            {
                FileStream filestream = null;
                try
                {
                    info.Refresh();

                    filestream = info.Open(FileMode.Open, FileAccess.ReadWrite);
                    byte[] bytes = System.Security.Cryptography.SHA1.Create().ComputeHash(filestream);
                    hashcode = BitConverter.ToString(bytes).Replace("-", "").ToLower();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (filestream != null)
                    {
                        filestream.Close();
                        filestream.Dispose();
                    }
                    filestream = null;
                }
            }
            return hashcode;
        }

        public static string GetFileHash(FileInfo info)
        {
            string hashcode = "";
            if (info.Exists)
            {
                FileStream filestream = null;

                try
                {
                    info.Refresh();

                    filestream = info.Open(FileMode.Open, FileAccess.ReadWrite);

                    byte[] bytes = System.Security.Cryptography.SHA1.Create().ComputeHash(filestream);
                    hashcode = BitConverter.ToString(bytes).Replace("-", "").ToLower();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (filestream != null)
                    {
                        filestream.Close();
                        filestream.Dispose();
                    }
                    filestream = null;
                }
            }
            return hashcode;
        }

        public static string GetFileHash(byte[] data)
        {
            string hashcode = "";
            try
            {
                byte[] bytes = System.Security.Cryptography.SHA1.Create().ComputeHash(data);
                hashcode = BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return hashcode;
        }

        #endregion

        #region 获取文本内容的hash值

        /// <summary>
        /// SHs the a1.
        /// </summary>
        /// <returns>The a1.</returns>
        /// <param name="text">Text.</param>
        public static string SHA1(string text)
        {
            byte[] cleanBytes = Encoding.Default.GetBytes(text);
            byte[] hashedBytes = System.Security.Cryptography.SHA1.Create().ComputeHash(cleanBytes);
            return BitConverter.ToString(hashedBytes).Replace("-", "");
        }

        public static byte[] SHAToBytes(string text)
        {
            byte[] cleanBytes = Encoding.Default.GetBytes(text);
            return System.Security.Cryptography.SHA1.Create().ComputeHash(cleanBytes);
        }

        #endregion

        #region Creates the name of the directory.

        /// <summary>
        /// Creates the name of the directory.
        /// </summary>
        /// <param name="filePath">File path.</param>
        public static void CreateDirectoryName(string filePath)
        {
            try
            {
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

            }
            catch (System.Exception e)
            {
                //to do
                Debug.Log("IO error." + e.Message);
            }
        }

        #endregion

        #region Gets the Directories.

        /// <summary>
        /// Gets the Directories.
        /// </summary>
        /// <returns>The directories.</returns>
        /// <param name="filePath">File path.</param>
        public static string[] GetDirectories(string filePath)
        {
            return Directory.GetDirectories(filePath);
        }

        #endregion

        #region Gets the Files.

        /// <summary>
        /// Gets the Files.
        /// </summary>
        /// <returns>The Files.</returns>
        /// <param name="filePath">File path.</param>
        public static string[] GetFiles(string filePath)
        {
            return Directory.GetFiles(filePath);
        }

        #endregion

        #region Gets the files in directories.

        /// <summary>
        /// Gets the files in directories.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="files">Files.</param>
        /// <param name="fileType">File type.</param>
        public static void GetFilesInDirectories(string path, ref List<string> files, string fileType, bool isEndsWith)
        {
            string[] filesT = FileManager.GetFiles(path);
            foreach (string item in filesT)
            {
                if (item.EndsWith(fileType) == isEndsWith && !item.EndsWith(".DS_Store"))
                {
                    files.Add(item);
                }
            }

            string[] pathsD = FileManager.GetDirectories(path);
            if (pathsD == null || pathsD.Length <= 0)
                return;

            //遍历所有的游戏对象
            foreach (string pathD in pathsD)
            {
                FileManager.GetFilesInDirectories(pathD, ref files, fileType, isEndsWith);
            }
        }

        #endregion

        #region Gets the file info.

        /// <summary>
        /// Gets the File info.
        /// </summary>
        /// <returns>The File info.</returns>
        /// <param name="filePath">File path.</param>
        public static FileInfo GetFileInfo(string filePath)
        {
            return new FileInfo(filePath);
        }

        #endregion

        #region Deletes directory.

        /// <summary>
        /// Deletes directory.
        /// </summary>
        /// <param name="path">Path.</param>
        public static void DeleteDirectory(string path)
        {
            if (FileManager.DirectoryExists(path))
            {
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    FileManager.DeleteFile(file);
                }
                string[] directorys = GetDirectories(path);
                foreach (string directory in directorys)
                {
                    FileManager.DeleteDirectory(directory);
                }
                Directory.Delete(path);
            }
        }

        #endregion

        #region Delete the file.

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="path">Path.</param>
        public static void DeleteFile(string path)
        {
            if (FileManager.Exists(path))
            {
                File.Delete(path);
            }
        }

        #endregion

        #region Copy

        /// <summary>
        /// Copy the specified sourceFileName and destFileName.
        /// </summary>
        /// <param name="sourceFileName">Source file name.</param>
        /// <param name="destFileName">Destination file name.</param>
        public static void Copy(string sourceFileName, string destFileName)
        {

            try
            {
                string dir = Path.GetDirectoryName(destFileName);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

            }
            catch (System.Exception e)
            {
                //to do
                Debug.Log("IO error." + e.Message);
            }
            File.Copy(sourceFileName, destFileName);
        }

        public static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    CopyDirectory(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        #endregion

        #region 加密或解密

        /// <summary>
        /// 加密.
        /// </summary>
        /// <returns>The to bytes.</returns>
        /// <param name="data">Data.</param>
        /// <param name="password">Password.</param>
        public static byte[] EncryptionToBytes(byte[] data, string password)
        {
            int version = 2;
            switch (version)
            {
                case 1:
                    return Encryption1(data);
                case 2:
                    return Encryption2(data, password);
                default:
                    return Encryption1(data);
            }
        }

        /// <summary>
        /// 加密.
        /// </summary>
        /// <returns>The to bytes.</returns>
        /// <param name="str">String.</param>
        /// <param name="password">Password.</param>
        public static byte[] EncryptionToBytes(string str, string password)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(str);

            return EncryptionToBytes(data, password);
        }

        /// <summary>
        /// 加密.
        /// </summary>
        /// <returns>The to string.</returns>
        /// <param name="data">Data.</param>
        /// <param name="password">Password.</param>
        public static string EncryptionToString(byte[] data, string password)
        {
            return System.Text.Encoding.UTF8.GetString(EncryptionToBytes(data, password));
        }

        /// <summary>
        /// 加密.
        /// </summary>
        /// <returns>The to string.</returns>
        /// <param name="str">String.</param>
        /// <param name="password">Password.</param>
        public static string EncryptionToString(string str, string password)
        {
            return System.Text.Encoding.UTF8.GetString(EncryptionToBytes(str, password));
        }

        /// <summary>
        /// 解密.
        /// </summary>
        /// <returns>The to bytes.</returns>
        /// <param name="data">Data.</param>
        /// <param name="password">Password.</param>
        public static byte[] DecryptionToBytes(byte[] data, string password = "ZF")
        {
            int version = data[0];
            switch (version)
            {
                case 1:
                    return Decryption1(data, 1);
                case 2:
                    return Decryption2(data, password);
                default:
                    return Decryption1(data, 0);
            }
        }


        /// <summary>
        /// 解密.
        /// </summary>
        /// <returns>The to string.</returns>
        /// <param name="data">Data.</param>
        /// <param name="password">Password.</param>
        public static string DecryptionToString(byte[] data, string password = "ZF")
        {
            data = DecryptionToBytes(data, password);
            return System.Text.Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// 解密.
        /// </summary>
        /// <returns>The to bytes.</returns>
        /// <param name="data">Data.</param>
        /// <param name="password">Password.</param>
        public static byte[] DecryptionToBytes(string data, string password = "ZF")
        {
            byte[] datas = System.Text.Encoding.UTF8.GetBytes(data);
            return DecryptionToBytes(datas, password);
        }


        /// <summary>
        /// 解密.
        /// </summary>
        /// <returns>The to string.</returns>
        /// <param name="data">Data.</param>
        /// <param name="password">Password.</param>
        public static string DecryptionToString(string data, string password = "ZF")
        {
            return System.Text.Encoding.UTF8.GetString(DecryptionToBytes(data, password));
        }

        #endregion

        #region 加密或解密 version: 0

        /// <summary>
        /// 加密.
        /// </summary>
        /// <param name="data">Data.</param>
        private static byte[] Encryption1(byte[] data)
        {
            byte[] dataNew = new byte[data.Length + 1];
            dataNew[0] = 1;
            for (int i = 0; i < data.Length; i++)
            {
                dataNew[i + 1] = data[i];
                dataNew[i + 1] ^= 0x43;
            }
            return dataNew;
        }

        /// <summary>
        /// 解密.
        /// </summary>
        /// <param name="data">Data.</param>
        private static byte[] Decryption1(byte[] data, int offset)
        {
            byte[] dataNew = new byte[data.Length - offset];
            for (int i = offset; i < data.Length; i++)
            {
                dataNew[i - offset] = data[i];
                dataNew[i - offset] ^= 0x43;
            }
            return dataNew;
        }

        /// <summary>
        /// 加密.
        /// </summary>
        /// <param name="data">Data.</param>
        private static byte[] Encryption2(byte[] data, string password = "ZF")
        {
            byte[] keys = FileManager.SHAToBytes(password);
            byte[] dataNew = new byte[data.Length + 1];
            dataNew[0] = 2;
            int keyLength = keys.Length;
            for (int i = 0; i < data.Length; i++)
            {
                dataNew[i + 1] = data[i];
                dataNew[i + 1] ^= keys[i & 15];
            }

            return dataNew;
        }

        /// <summary>
        /// 解密.
        /// </summary>
        /// <param name="data">Data.</param>
        private static byte[] Decryption2(byte[] data, string password = "ZF")
        {
            byte[] keys = FileManager.SHAToBytes(password);
            byte[] dataNew = new byte[data.Length - 1];
            int keyLength = keys.Length;
            for (int i = 1; i < data.Length; i++)
            {
                dataNew[i - 1] = data[i];
                dataNew[i - 1] ^= keys[i - 1 & 15];
            }
            return dataNew;
        }

        #endregion

        #region 解析CSV.

        /// <summary>
        /// 解析CSV.
        /// </summary>
        /// <returns>数据列表.</returns>
        /// <param name="data">数据源.</param>
        /// <typeparam name="T">对象类型.</typeparam>
        public static List<T> CsvDecod<T>(string data) where T : class, new()
        {
            byte[] bytes = System.Text.Encoding.Default.GetBytes(data);
            return CsvDecod<T>(bytes);
        }

        /// <summary>
        /// 解析CSV.
        /// </summary>
        /// <returns>数据列表.</returns>
        /// <param name="bytes">数据源.</param>
        /// <typeparam name="T">对象类型.</typeparam>
        public static List<T> CsvDecod<T>(byte[] bytes) where T : class, new()
        {
            List<T> list = new List<T>();
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    CsvContext mCsvContext = new CsvContext();
                    list.AddRange(mCsvContext.Read<T>(reader));
                }
            }
            return list;
        }

        #endregion


        public static string AssetDataPathToResourcesPath(string path)
        {
            if (path.IndexOf("/Resources/", StringComparison.CurrentCulture) != -1)
            {
                string resourcesPath = path.Substring(path.IndexOf("/Resources/", StringComparison.CurrentCulture) + "/Resources/".Length);
                resourcesPath = resourcesPath.Remove(resourcesPath.LastIndexOf(".", StringComparison.CurrentCulture));
                return resourcesPath;
            }
            return path;
        }

        public static string GetFileNameFromPath(string path){
            if (string.IsNullOrEmpty(path))
                return null;
            string fileName = path;
            if(path.LastIndexOf("/",StringComparison.CurrentCulture)!=-1){
                int index = path.LastIndexOf("/", StringComparison.CurrentCulture) + 1;
                fileName = path.Substring(index);
            }
            return fileName;
        }

        public static string GetFileMain(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;
            if (fileName.LastIndexOf(".", StringComparison.CurrentCulture) != -1)
            {
                int index = fileName.LastIndexOf(".", StringComparison.CurrentCulture);
                fileName = fileName.Substring(0,index);
            }
            return fileName;
        }

        public static string GetFilePattern(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;
            if (fileName.LastIndexOf(".", StringComparison.CurrentCulture) != -1)
            {
                int index = fileName.LastIndexOf(".", StringComparison.CurrentCulture) + 1;
                fileName = fileName.Substring(index);
            }
            return fileName;
        }

    }
}