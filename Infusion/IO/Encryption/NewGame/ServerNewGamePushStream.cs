namespace Infusion.IO.Encryption.NewGame
{
    internal sealed class ServerNewGamePushStream : NewGamePushStream
    {
        public ServerNewGamePushStream() : base(null) { }
        public ServerNewGamePushStream(byte[] cryptoKey) : base(new NewGameCrypt(cryptoKey).Encrypt) { }
    }
}