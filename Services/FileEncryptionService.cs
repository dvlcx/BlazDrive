using CryptoNet;

namespace BlazDrive.Services
{
    public class FileEncryptionService
    {
        public byte[] EncryptFile(byte[] file)
        {
            ICryptoNet encryptClient = new CryptoNetAes("sex");
            var encrypt = encryptClient.EncryptFromBytes(file);
            return encrypt;
        }

        public byte[] DecryptFile(byte[] file)
        {
            ICryptoNet decryptClient = new CryptoNetAes("sex");
            var decrypt = decryptClient.DecryptToBytes(file);
            return decrypt;
        }
    }
}