namespace UltimaRX.IO
{
    internal class TwofishEncryption : TwofishBase
    {
        // not worked out this property yet - placing break points here just don't get caught.

        // I normally set this to false when block encrypting so that I can work on one block at a time
        // but for compression and stream type ciphers this can be set to true so that you get all the data

        private readonly EncryptionDirection encryptionDirection;

        public TwofishEncryption(int keyLen, byte[] key, byte[] iv, CipherMode cMode, EncryptionDirection direction)
        {
            // convert our key into an array of ints
            for (var i = 0; i < key.Length/4; i++)
                Key[i] = (uint) (key[i*4 + 3] << 24) | (uint) (key[i*4 + 2] << 16) | (uint) (key[i*4 + 1] << 8) |
                         key[i*4 + 0];

            cipherMode = cMode;

            // we only need to convert our IV if we are using CBC
            if (cipherMode == CipherMode.CBC)
            {
                for (var i = 0; i < 4; i++)
                    IV[i] = (uint) (iv[i*4 + 3] << 24) | (uint) (iv[i*4 + 2] << 16) | (uint) (iv[i*4 + 1] << 8) |
                            iv[i*4 + 0];
            }

            encryptionDirection = direction;
            reKey(keyLen, ref Key);
        }

        public bool CanReuseTransform { get; } = true;

        public bool CanTransformMultipleBlocks { get; } = false;

        public int InputBlockSize
        {
            get { return inputBlockSize; }
        }

        public int OutputBlockSize
        {
            get { return outputBlockSize; }
        }

        // need to have this method due to IDisposable - just can't think of a reason to use it for in this class
        public void Dispose()
        {
        }

        /// <summary>
        ///     Transform a block depending on whether we are encrypting or decrypting
        /// </summary>
        /// <param name="inputBuffer"></param>
        /// <param name="inputOffset"></param>
        /// <param name="inputCount"></param>
        /// <param name="outputBuffer"></param>
        /// <param name="outputOffset"></param>
        /// <returns></returns>
        public int TransformBlock(
            byte[] inputBuffer,
            int inputOffset,
            int inputCount,
            byte[] outputBuffer,
            int outputOffset
        )
        {
            var x = new uint[4];

            // load it up
            for (var i = 0; i < 4; i++)
            {
                x[i] = (uint) (inputBuffer[i*4 + 3 + inputOffset] << 24) |
                       (uint) (inputBuffer[i*4 + 2 + inputOffset] << 16) |
                       (uint) (inputBuffer[i*4 + 1 + inputOffset] << 8) | inputBuffer[i*4 + 0 + inputOffset];
            }

            if (encryptionDirection == EncryptionDirection.Encrypting)
                blockEncrypt(ref x);
            else
                blockDecrypt(ref x);

            // load it up
            for (var i = 0; i < 4; i++)
            {
                outputBuffer[i*4 + 0 + outputOffset] = b0(x[i]);
                outputBuffer[i*4 + 1 + outputOffset] = b1(x[i]);
                outputBuffer[i*4 + 2 + outputOffset] = b2(x[i]);
                outputBuffer[i*4 + 3 + outputOffset] = b3(x[i]);
            }


            return inputCount;
        }

        public byte[] TransformFinalBlock(
            byte[] inputBuffer,
            int inputOffset,
            int inputCount
        )
        {
            byte[] outputBuffer; // = new byte[0];

            if (inputCount > 0)
            {
                outputBuffer = new byte[16]; // blocksize
                var x = new uint[4];

                // load it up
                for (var i = 0; i < 4; i++) // should be okay as we have already said to pad with zeros
                {
                    x[i] = (uint) (inputBuffer[i*4 + 3 + inputOffset] << 24) |
                           (uint) (inputBuffer[i*4 + 2 + inputOffset] << 16) |
                           (uint) (inputBuffer[i*4 + 1 + inputOffset] << 8) | inputBuffer[i*4 + 0 + inputOffset];
                }

                if (encryptionDirection == EncryptionDirection.Encrypting)
                    blockEncrypt(ref x);
                else
                    blockDecrypt(ref x);

                // load it up
                for (var i = 0; i < 4; i++)
                {
                    outputBuffer[i*4 + 0] = b0(x[i]);
                    outputBuffer[i*4 + 1] = b1(x[i]);
                    outputBuffer[i*4 + 2] = b2(x[i]);
                    outputBuffer[i*4 + 3] = b3(x[i]);
                }
            }
            else
                outputBuffer = new byte[0];
                    // the .NET framework doesn't like it if you return null - this calms it down

            return outputBuffer;
        }
    }
}