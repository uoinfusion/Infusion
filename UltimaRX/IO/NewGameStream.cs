using System;
using System.IO;

namespace UltimaRX.IO
{
    public class NewGameStream : Stream
    {
        private static readonly byte[] sm_bData =
        {
            0x05, 0x92, 0x66, 0x23, 0x67, 0x14, 0xE3,
            0x62, 0xDC, 0x60, 0x8C, 0xD6, 0xFE, 0x7C, 0x25, 0x69
        };

        public IPullStream BaseStream { get; set; }
        private readonly byte[] m_subData3 = new byte[256];
        private uint dwIndex;
        private TwofishEncryption encTwofish;
        private int m_pos;
        private byte[] cryptoKey;

        public NewGameStream(IPullStream baseStream, byte[] cryptoKey)
        {
            BaseStream = baseStream;
            this.cryptoKey = cryptoKey;

            Initialize();
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
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

        private static readonly byte[] key =
        {
            0x7f, 0x00, 0x00, 0x01, 0x7f, 0x00, 0x00, 0x01, 0x7f, 0x00, 0x00, 0x01,
            0x7f, 0x00, 0x00, 0x01
        };

        private void Initialize()
        {
            const byte encryptionKeyRepetitionCount = 4;
            byte[] keyBuffer = new byte[cryptoKey.Length * encryptionKeyRepetitionCount];
            for (int i = 0; i < encryptionKeyRepetitionCount; i++)
            {
                cryptoKey.CopyTo(keyBuffer, i * encryptionKeyRepetitionCount);
            }

            var tmpBuff = new byte[256];
            encTwofish = new TwofishEncryption(0x80, key, null, CipherMode.ECB,
                TwofishBase.EncryptionDirection.Encrypting);
            for (var i = 0; i < 256; i++)
                m_subData3[i] = (byte)i;

            Encrypt(m_subData3, tmpBuff, 256);

            for (var i = 0; i < 256; i++)
            {
                m_subData3[i] = tmpBuff[i];
            }

            m_pos = 0;
            dwIndex = 0;
        }

        private void Decrypt(byte[] input, byte[] output, long len)
        {
            var dwTmpIndex = dwIndex;
            for (var i = 0; i < len; i++)
            {
                output[i] = (byte)(input[i] ^ sm_bData[dwTmpIndex % 16]);
                dwTmpIndex++;
            }
            dwIndex = dwTmpIndex;
        }

        private void Encrypt(byte[] input, byte[] output, long len)
        {
            var tmpBuff = new byte[0x100];

            for (var i = 0; i < len; i++)
            {
                if (m_pos == 0x100)
                {
                    Encrypt(m_subData3, tmpBuff, 0x100);
                    //					encTwofish.TransformBlock(m_subData3, 0, 0x800, tmpBuff, 0);

                    for (var j = 0; j < 0x100; j++)
                    {
                        m_subData3[j] = tmpBuff[j];
                    }
                    m_pos = 0;
                }
                output[i] = (byte)(input[i] ^ m_subData3[m_pos++]);
            }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var encrypted = new byte[count + 1];
            var encryptedCount = BaseStream.Read(encrypted, 0, count);

            Decrypt(encrypted, buffer, count);

            return encryptedCount;
        }

        public override int ReadByte()
        {
            var encrypted = new byte[1];
            int value = BaseStream.ReadByte();
            if (value < 0 || value > 255)
            {
                throw new EndOfStreamException();
            }

            encrypted[0] = (byte)value;

            var buffer = new byte[1];
            Decrypt(encrypted, buffer, 1);

            return buffer[0];
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}