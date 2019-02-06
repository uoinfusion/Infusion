namespace Infusion.IO.Encryption.NewGame
{

    internal sealed class ServerNewGamePullStream : NewGamePullStream
    {
        public ServerNewGamePullStream() : base(null) { }

        public ServerNewGamePullStream(byte[] cryptoKey) : base(new NewGameCrypt(cryptoKey).Decrypt) { }
    }
}