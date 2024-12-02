using CryptoNet;

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
            var nkey = new string(key.Take(43).ToArray()) + "=";
            var iv = new string(key.Skip(30).Take(22).ToArray()) + "==";
            var c = $"<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<AesKeyValue xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\n  <key>{nkey}</key>\n  <iv>{iv}</iv>\n</AesKeyValue>";
            return new CryptoNetAes(c);
        }
    }
}