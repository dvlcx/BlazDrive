using CryptoNet;
using BlazDrive.Utils;

namespace BlazDrive.Services
{
    public class FileEncryptionService
    {
        private AccountInfoService _accountInfo;
        public FileEncryptionService(AccountInfoService accountInfoService)
        {
            _accountInfo = accountInfoService;
        }

        public async Task<byte[]> EncryptFile(byte[] file)
        {   
            var key = await _accountInfo.GetEncryptionKey();
            CryptoNetAes encryptClient = new CryptoNetAes(key);
            var encrypt = encryptClient.EncryptFromBytes(file);
            return encrypt;
        }

        public async Task<byte[]> DecryptFile(byte[] file)
        {
            var key = await _accountInfo.GetEncryptionKey();
            ICryptoNet decryptClient = new CryptoNetAes(key);
            var decrypt = decryptClient.DecryptToBytes(file);
            return decrypt;
        }

        public byte[] EncryptFile(byte[] file, string key)
        {   
            var client = this.CreateCryptoClient(key);
            var encrypt = client.EncryptFromBytes(file);
            return encrypt;
        }

        public byte[] DecryptFile(byte[] file, string key)
        {
            var client = this.CreateCryptoClient(key);
            var decrypt = client.DecryptToBytes(file);
            return decrypt;
        }

        private ICryptoNet CreateCryptoClient(string key)
        {
            var c = EncryptionUtils.FormatAesKey(key);
            return new CryptoNetAes(c);
        }
    }
}