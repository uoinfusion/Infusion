namespace Infusion.IO.Encryption.NewGame
{
    internal sealed class ClientNewGamePushStream : NewGamePushStream
    {
        public ClientNewGamePushStream() : base(null) { }
        public ClientNewGamePushStream(byte[] cryptoKey) : base(new NewGameCrypt(cryptoKey).Decrypt) { }
    }
}