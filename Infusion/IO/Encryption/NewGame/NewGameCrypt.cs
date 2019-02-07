using System.Linq;

namespace Infusion.IO.Encryption.NewGame
{
    internal sealed class NewGameCrypt
    {
        private static readonly byte[] sm_bData =
        {
            0x05, 0x92, 0x66, 0x23, 0x67, 0x14, 0xE3,
            0x62, 0xDC, 0x60, 0x8C, 0xD6, 0xFE, 0x7C, 0x25, 0x69
        };

        private readonly byte[] key;
        private readonly byte[] cryptoKey;
        private readonly byte[] m_subData3 = new byte[256];
        private uint dwIndex;
        private TwofishEncryption encTwofish;
        private int m_pos;

        public NewGameCrypt(byte[] cryptoKey)
        {
            key = Enumerable.Repeat(cryptoKey, 4).SelectMany(x => x).ToArray();
            this.cryptoKey = cryptoKey;
            Initialize();
        }

        private void Encrypt(byte[] input, byte[] output, int len)
        {
            var offset = 0;
            while (len > 0)
            {
                if (len <= 16)
                {
                    encTwofish.TransformBlock(input, offset, len * 8, output, offset);
                    len = 0;
                }
                else
                {
                    encTwofish.TransformBlock(input, offset, len * 8, output, offset);
                    len -= 16;
                    offset += 16;
                }
            }
        }

        private void Initialize()
        {
            const byte encryptionKeyRepetitionCount = 4;
            var keyBuffer = new byte[cryptoKey.Length * encryptionKeyRepetitionCount];
            for (var i = 0; i < encryptionKeyRepetitionCount; i++)
                cryptoKey.CopyTo(keyBuffer, i * encryptionKeyRepetitionCount);

            var tmpBuff = new byte[256];
            encTwofish = new TwofishEncryption(0x80, key, null, CipherMode.ECB,
                TwofishBase.EncryptionDirection.Encrypting);
            for (var i = 0; i < 256; i++)
                m_subData3[i] = (byte)i;

            Encrypt(m_subData3, tmpBuff, 256);

            for (var i = 0; i < 256; i++)
                m_subData3[i] = tmpBuff[i];

            m_pos = 0;
            dwIndex = 0;
        }

        public void InitializeMD5()
        {
            using (System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                var hash = md5.ComputeHash(m_subData3, 0, m_subData3.Length);
                hash.CopyTo(sm_bData,0);
            }
        }

        public void Decrypt(byte[] input, byte[] output, long len)
        {
            var dwTmpIndex = dwIndex;
            for (var i = 0; i < len; i++)
            {
                output[i] = (byte)(input[i] ^ sm_bData[dwTmpIndex % 16]);
                dwTmpIndex++;
            }
            dwIndex = dwTmpIndex;
        }

        public void Encrypt(byte[] input, byte[] output, long len)
        {
            var tmpBuff = new byte[0x100];

            for (var i = 0; i < len; i++)
            {
                if (m_pos == 0x100)
                {
                    Encrypt(m_subData3, tmpBuff, 0x100);

                    for (var j = 0; j < 0x100; j++)
                        m_subData3[j] = tmpBuff[j];
                    m_pos = 0;
                }
                output[i] = (byte)(input[i] ^ m_subData3[m_pos++]);
            }
        }
    }
}