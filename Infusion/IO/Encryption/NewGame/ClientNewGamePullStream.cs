namespace Infusion.IO.Encryption.NewGame
{
    internal sealed class ClientNewGamePullStream : NewGamePullStream
    {
        public ClientNewGamePullStream() : base(null) { }

        public ClientNewGamePullStream(byte[] cryptoKey) : base(new NewGameCrypt(cryptoKey).Encrypt) { }
    }
}