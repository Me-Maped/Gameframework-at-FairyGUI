using System;
using System.IO;
using YooAsset;

namespace GameFramework.Resource
{
    internal partial class ResourceManager
    {
        /// <summary>
        /// 资源文件解密服务类。
        /// </summary>
        private class GameDecryptionServices : IDecryptionServices
        {
            public ulong LoadFromFileOffset(DecryptFileInfo fileInfo)
            {
                return 32;
            }

            public byte[] LoadFromMemory(DecryptFileInfo fileInfo)
            {
                throw new NotImplementedException();
            }

            public Stream LoadFromStream(DecryptFileInfo fileInfo)
            {
                BundleStream bundleStream = new BundleStream(fileInfo.FilePath, FileMode.Open);
                return bundleStream;
            }

            public uint GetManagedReadBufferSize()
            {
                return 1024;
            }
        }

        /// <summary>
        /// 内置文件查询服务类。
        /// </summary>
        private class GameBuildinQueryServices : IBuildinQueryServices
        {
            public bool QueryStreamingAssets(string packageName, string fileName)
            {
                return StreamingAssetsHelper.FileExists(packageName, fileName);
            }
        }
        
        private class GameDeliveryQueryServices : IDeliveryQueryServices
        {
            public bool QueryDeliveryFiles(string packageName, string fileName)
            {
                return false;
            }

            public DeliveryFileInfo GetDeliveryFileInfo(string packageName, string fileName)
            {
                throw new NotImplementedException();
            }
        }
        
        private class GameRemoteServices : IRemoteServices
        {
            private string m_RemoteServerURL;
            private string m_RemoteFallbackServerURL;
            public GameRemoteServices(string remoteServerURL,string remoteFallbackServerURL)
            {
                m_RemoteServerURL = remoteServerURL;
                m_RemoteFallbackServerURL = remoteFallbackServerURL;
            }
            
            public string GetRemoteMainURL(string fileName)
            {
                return Utility.Text.Format("{0}/{1}",m_RemoteServerURL,fileName);
            }

            public string GetRemoteFallbackURL(string fileName)
            {
                return Utility.Text.Format("{0}/{1}",m_RemoteFallbackServerURL,fileName);
            }
        }
    }

    public class BundleStream : FileStream
    {
        public const byte KEY = 64;

        public BundleStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync) : base(path, mode, access, share, bufferSize, useAsync)
        {
        }

        public BundleStream(string path, FileMode mode) : base(path, mode)
        {
        }

        public override int Read(byte[] array, int offset, int count)
        {
            var index = base.Read(array, offset, count);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] ^= KEY;
            }

            return index;
        }
    }
}