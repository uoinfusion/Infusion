namespace Infusion.IO.Encryption.NewGame
{
    internal sealed class ClientNewGamePullStream : NewGamePullStream
    {
        public ClientNewGamePullStream() { }

        public ClientNewGamePullStream(byte[] cryptoKey) {
            var crypt = new NewGameCrypt(cryptoKey);
            encrypt = crypt.Encrypt;
        }
    }
}