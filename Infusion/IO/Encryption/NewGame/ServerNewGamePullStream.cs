namespace Infusion.IO.Encryption.NewGame
{

    internal sealed class ServerNewGamePullStream : NewGamePullStream
    {
        public ServerNewGamePullStream() { }

        public ServerNewGamePullStream(byte[] cryptoKey)
        {
            var crypt = new NewGameCrypt(cryptoKey);
            crypt.InitializeMD5();
            this.encrypt = crypt.Decrypt;
        }
    }
}