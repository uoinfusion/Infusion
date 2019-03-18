using System.ComponentModel;

namespace Infusion
{
    public enum EncryptionSetup
    {
        [Description("Autodetect")]
        Autodetect,

        [Description("Unencrypted Client -> Encrypted Server")]
        EncryptedServer,
    }
}
