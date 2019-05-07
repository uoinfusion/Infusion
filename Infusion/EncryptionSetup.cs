using System.ComponentModel;

namespace Infusion
{
    public enum EncryptionSetup
    {
        [Description("Autodetect")]
        Autodetect,

        [Description("Add encryption to client")]
        EncryptedServer,

        [Description("Remove encryption from client")]
        EncryptedClient,
    }
}
