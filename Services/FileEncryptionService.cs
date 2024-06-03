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
            // ICryptoNet cryptoNet = new CryptoNetAes();
            // var key = cryptoNet.ExportKey();
            var key = await _accountInfo.GetEncryptionKey();
            CryptoNetAes encryptClient = new CryptoNetAes(key);
            encryptClient.ExportKey();
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
    }
}