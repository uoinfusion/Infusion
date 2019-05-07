namespace Infusion.IO.Encryption.NewGame
{

    internal sealed class ServerNewGamePullStream : NewGamePullStream
    {
        public ServerNewGamePullStream() { }

        public ServerNewGamePullStream(byte[] cryptoKey)
        {
            var crypt = new NewGameCrypt(cryptoKey);
            this.encrypt = crypt.Decrypt;
        }
    }
}