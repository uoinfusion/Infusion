using Infusion.IO.Encryption.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Desktop.Launcher
{
    public sealed class EncryptionVersion
    {
        public static EncryptionVersion[] Versions { get; } =
        {
            new EncryptionVersion("7.0.74", new LoginEncryptionKey(0x3F89695D, 0xABCCA27F)),
            new EncryptionVersion("7.0.73", new LoginEncryptionKey(0x2042036D, 0xA5D1BE7F)),
            new EncryptionVersion("7.0.72", new LoginEncryptionKey(0x207B217D, 0xA5C7527F)),
            new EncryptionVersion("7.0.71", new LoginEncryptionKey(0x203CC38D, 0xA5FDFE7F)),
            new EncryptionVersion("7.0.70", new LoginEncryptionKey(0x20E5E99D, 0xA598227F)),
            new EncryptionVersion("7.0.69", new LoginEncryptionKey(0x20AE93AD, 0xA586DE7F)),
            new EncryptionVersion("7.0.68", new LoginEncryptionKey(0x215781BD, 0xA57D127F)),
            new EncryptionVersion("7.0.67", new LoginEncryptionKey(0x2118B3CD, 0xA5535E7F)),
            new EncryptionVersion("7.0.66", new LoginEncryptionKey(0x21C1A9DD, 0xA521227F)),
            new EncryptionVersion("7.0.65", new LoginEncryptionKey(0x218AA3ED, 0xA50F7E7F)),
            new EncryptionVersion("7.0.64", new LoginEncryptionKey(0x21B3A1FD, 0xA515527F)),
            new EncryptionVersion("7.0.63", new LoginEncryptionKey(0x2244A40D, 0xA484BE7F)),
            new EncryptionVersion("7.0.62", new LoginEncryptionKey(0x221DAE1D, 0xA4A6A27F)),
            new EncryptionVersion("7.0.61", new LoginEncryptionKey(0x22D6B42D, 0xA4D89E7F)),
            new EncryptionVersion("7.0.60", new LoginEncryptionKey(0x22AF863D, 0xA4E2127F)),
            new EncryptionVersion("7.0.59", new LoginEncryptionKey(0x2360944D, 0xA40D1E7F)),
            new EncryptionVersion("7.0.58", new LoginEncryptionKey(0x2339EE5D, 0xA41FA27F)),
            new EncryptionVersion("7.0.57", new LoginEncryptionKey(0x23F2C46D, 0xA47E3E7F)),
            new EncryptionVersion("7.0.56", new LoginEncryptionKey(0x23CB267D, 0xA469527F)),
            new EncryptionVersion("7.0.55", new LoginEncryptionKey(0x238C048D, 0xA4527E7F)),
            new EncryptionVersion("7.0.54", new LoginEncryptionKey(0x24556E9D, 0xA7BB227F)),
            new EncryptionVersion("7.0.53", new LoginEncryptionKey(0x241E54AD, 0xA7945E7F)),
            new EncryptionVersion("7.0.52", new LoginEncryptionKey(0x24E686BD, 0xA715127F)),
            new EncryptionVersion("7.0.51", new LoginEncryptionKey(0x24AFF4CD, 0xA736DE7F)),
            new EncryptionVersion("7.0.50", new LoginEncryptionKey(0x25702EDD, 0xA7D6227F)),
            new EncryptionVersion("7.0.49", new LoginEncryptionKey(0x253964ED, 0xA7F7FE7F)),
            new EncryptionVersion("7.0.48", new LoginEncryptionKey(0x2501A6FD, 0xA7F7527F)),
            new EncryptionVersion("7.0.47", new LoginEncryptionKey(0x25CAE50D, 0xA79B3E7F)),
            new EncryptionVersion("7.0.46", new LoginEncryptionKey(0x25932F1D, 0xA7B3A27F)),
            new EncryptionVersion("7.0.45", new LoginEncryptionKey(0x2644752D, 0xA66A1E7F)),
            new EncryptionVersion("7.0.44", new LoginEncryptionKey(0x263C873D, 0xA652127F)),
            new EncryptionVersion("7.0.43", new LoginEncryptionKey(0x26F5D54D, 0xA63A9E7F)),
            new EncryptionVersion("7.0.42", new LoginEncryptionKey(0x26AE6F5D, 0xA612A27F)),
            new EncryptionVersion("7.0.41", new LoginEncryptionKey(0x2766856D, 0xA6EABE7F)),
            new EncryptionVersion("7.0.40", new LoginEncryptionKey(0x275F277D, 0xA6F3527F)),
            new EncryptionVersion("7.0.39", new LoginEncryptionKey(0x2710458D, 0xA6DAFE7F)),
            new EncryptionVersion("7.0.38", new LoginEncryptionKey(0x27C8EF9D, 0xA6B2227F)),
            new EncryptionVersion("7.0.37", new LoginEncryptionKey(0x278115AD, 0xA695DE7F)),
            new EncryptionVersion("7.0.36", new LoginEncryptionKey(0x287987BD, 0xA115127F)),
            new EncryptionVersion("7.0.35", new LoginEncryptionKey(0x283235CD, 0xA1345E7F)),
            new EncryptionVersion("7.0.34", new LoginEncryptionKey(0x28EAAFDD, 0xA157227F)),
            new EncryptionVersion("7.0.33", new LoginEncryptionKey(0x28A325ED, 0xA1767E7F)),
            new EncryptionVersion("7.0.32", new LoginEncryptionKey(0x289BA7FD, 0xA169527F)),
            new EncryptionVersion("7.0.31", new LoginEncryptionKey(0x295C260D, 0xA197BE7F)),
            new EncryptionVersion("7.0.30", new LoginEncryptionKey(0x2904AC1D, 0xA1BCA27F)),
            new EncryptionVersion("7.0.29", new LoginEncryptionKey(0x29CD362D, 0xA1D59E7F)),
            new EncryptionVersion("7.0.28", new LoginEncryptionKey(0x29B5843D, 0xA1EA127F)),
            new EncryptionVersion("7.0.27", new LoginEncryptionKey(0x2A7E164D, 0xA0081E7F)),
            new EncryptionVersion("7.0.26", new LoginEncryptionKey(0x2A26EC5D, 0xA019A27F)),
            new EncryptionVersion("7.0.25", new LoginEncryptionKey(0x2AEF466D, 0xA07F3E7F)),
            new EncryptionVersion("7.0.24", new LoginEncryptionKey(0x2AD7247D, 0xA065527F)),
            new EncryptionVersion("7.0.23", new LoginEncryptionKey(0x2A9F868D, 0xA0437E7F)),
            new EncryptionVersion("7.0.22", new LoginEncryptionKey(0x2B406C9D, 0xA0A1227F)),
            new EncryptionVersion("7.0.21", new LoginEncryptionKey(0x2B08D6AD, 0xA0875E7F)),
            new EncryptionVersion("7.0.20", new LoginEncryptionKey(0x2BF084BD, 0xA0FD127F)),
            new EncryptionVersion("7.0.19", new LoginEncryptionKey(0x2BB976CD, 0xA0DBDE7F)),
            new EncryptionVersion("7.0.18", new LoginEncryptionKey(0x2C612CDD, 0xA328227F)),
            new EncryptionVersion("7.0.17", new LoginEncryptionKey(0x2C29E6ED, 0xA30EFE7F)),
            new EncryptionVersion("7.0.16", new LoginEncryptionKey(0x2C11A4FD, 0xA313527F)),
            new EncryptionVersion("7.0.15", new LoginEncryptionKey(0x2CDA670D, 0xA3723E7F)),
            new EncryptionVersion("7.0.14", new LoginEncryptionKey(0x2C822D1D, 0xA35DA27F)),
            new EncryptionVersion("7.0.13", new LoginEncryptionKey(0x2D4AF72D, 0xA3B71E7F)),
            new EncryptionVersion("7.0.12", new LoginEncryptionKey(0x2D32853D, 0xA38A127F)),
            new EncryptionVersion("7.0.11", new LoginEncryptionKey(0x2DFB574D, 0xA3ED9E7F)),
            new EncryptionVersion("7.0.10", new LoginEncryptionKey(0x2DA36D5D, 0xA3C0A27F)),
            new EncryptionVersion("7.0.9", new LoginEncryptionKey(0x2E6B076D, 0xA223BE7F)),
            new EncryptionVersion("7.0.8", new LoginEncryptionKey(0x2E53257D, 0xA23F527F)),
            new EncryptionVersion("7.0.7", new LoginEncryptionKey(0x2E1BC78D, 0xA21BFE7F)),
            new EncryptionVersion("7.0.6", new LoginEncryptionKey(0x2EC3ED9D, 0xA274227F)),
            new EncryptionVersion("7.0.5", new LoginEncryptionKey(0x2E8B97AD, 0xA250DE7F)),
            new EncryptionVersion("7.0.4", new LoginEncryptionKey(0x2F7385BD, 0xA2AD127F)),
            new EncryptionVersion("7.0.3", new LoginEncryptionKey(0x2F3BB7CD, 0xA2895E7F)),
            new EncryptionVersion("7.0.2", new LoginEncryptionKey(0x2FE3ADDD, 0xA2E5227F)),
            new EncryptionVersion("7.0.1", new LoginEncryptionKey(0x2FABA7ED, 0xA2C17E7F)),
            new EncryptionVersion("7.0.0", new LoginEncryptionKey(0x2F93A5FD, 0xA2DD527F)),
            new EncryptionVersion("6.0.14", new LoginEncryptionKey(0x2C022D1D, 0xA31DA27F)),
            new EncryptionVersion("6.0.13", new LoginEncryptionKey(0x2DCAF72D, 0xA3F71E7F)),
            new EncryptionVersion("6.0.12", new LoginEncryptionKey(0x2DB2853D, 0xA3CA127F)),
            new EncryptionVersion("6.0.11", new LoginEncryptionKey(0x2D7B574D, 0xA3AD9E7F)),
            new EncryptionVersion("6.0.10", new LoginEncryptionKey(0x2D236D5D, 0xA380A27F)),
            new EncryptionVersion("6.0.9", new LoginEncryptionKey(0x2EEB076D, 0xA263BE7F)),
            new EncryptionVersion("6.0.8", new LoginEncryptionKey(0x2ED3257D, 0xA27F527F)),
            new EncryptionVersion("6.0.7", new LoginEncryptionKey(0x2E9BC78D, 0xA25BFE7F)),
            new EncryptionVersion("6.0.6", new LoginEncryptionKey(0x2E43ED9D, 0xA234227F)),
            new EncryptionVersion("6.0.5", new LoginEncryptionKey(0x2E0B97AD, 0xA210DE7F)),
            new EncryptionVersion("6.0.4", new LoginEncryptionKey(0x2FF385BD, 0xA2ED127F)),
            new EncryptionVersion("6.0.3", new LoginEncryptionKey(0x2FBBB7CD, 0xA2C95E7F)),
            new EncryptionVersion("6.0.2", new LoginEncryptionKey(0x2F63ADDD, 0xA2A5227F)),
            new EncryptionVersion("6.0.1", new LoginEncryptionKey(0x2F2BA7ED, 0xA2817E7F)),
            new EncryptionVersion("6.0.0", new LoginEncryptionKey(0x2F13A5FD, 0xA29D527F)),
            new EncryptionVersion("5.0.9", new LoginEncryptionKey(0x2F6B076D, 0xA2A3BE7F)),
            new EncryptionVersion("5.0.8", new LoginEncryptionKey(0x2F53257D, 0xA2BF527F)),
            new EncryptionVersion("5.0.7", new LoginEncryptionKey(0x2F1BC78D, 0xA29BFE7F)),
            new EncryptionVersion("5.0.6", new LoginEncryptionKey(0x2FC3ED9D, 0xA2F4227F)),
            new EncryptionVersion("5.0.5", new LoginEncryptionKey(0x2F8B97AD, 0xA2D0DE7F)),
            new EncryptionVersion("5.0.4", new LoginEncryptionKey(0x2E7385BD, 0xA22D127F)),
            new EncryptionVersion("5.0.3", new LoginEncryptionKey(0x2E3BB7CD, 0xA2095E7F)),
            new EncryptionVersion("5.0.2", new LoginEncryptionKey(0x2EE3ADDD, 0xA265227F)),
            new EncryptionVersion("5.0.1", new LoginEncryptionKey(0x2EABA7ED, 0xA2417E7F)),
            new EncryptionVersion("5.0.0", new LoginEncryptionKey(0x2E93A5FD, 0xA25D527F)),
            new EncryptionVersion("4.0.11", new LoginEncryptionKey(0x2C7B574D, 0xA32D9E7F)),
            new EncryptionVersion("4.0.10", new LoginEncryptionKey(0x2C236D5D, 0xA300A27F)),
            new EncryptionVersion("4.0.9", new LoginEncryptionKey(0x2FEB076D, 0xA2E3BE7F)),
            new EncryptionVersion("4.0.8", new LoginEncryptionKey(0x2FD3257D, 0xA2FF527F)),
            new EncryptionVersion("4.0.7", new LoginEncryptionKey(0x2F9BC78D, 0xA2DBFE7F)),
            new EncryptionVersion("4.0.6", new LoginEncryptionKey(0x2F43ED9D, 0xA2B4227F)),
            new EncryptionVersion("4.0.5", new LoginEncryptionKey(0x2F0B97AD, 0xA290DE7F)),
            new EncryptionVersion("4.0.4", new LoginEncryptionKey(0x2EF385BD, 0xA26D127F)),
            new EncryptionVersion("4.0.3", new LoginEncryptionKey(0x2EBBB7CD, 0xA2495E7F)),
            new EncryptionVersion("4.0.2", new LoginEncryptionKey(0x2E63ADDD, 0xA225227F)),
            new EncryptionVersion("4.0.1", new LoginEncryptionKey(0x2E2BA7ED, 0xA2017E7F)),
            new EncryptionVersion("4.0.0", new LoginEncryptionKey(0x2E13A5FD, 0xA21D527F)),
            new EncryptionVersion("3.0.8", new LoginEncryptionKey(0x2C53257D, 0xA33F527F)),
            new EncryptionVersion("3.0.7", new LoginEncryptionKey(0x2C1BC78D, 0xA31BFE7F)),
            new EncryptionVersion("3.0.6", new LoginEncryptionKey(0x2CC3ED9D, 0xA374227F)),
            new EncryptionVersion("3.0.5", new LoginEncryptionKey(0x2C8B97AD, 0xA350DE7F)),
            new EncryptionVersion("3.0.4", new LoginEncryptionKey(0x2D7385BD, 0xA3AD127F)),
            new EncryptionVersion("3.0.3", new LoginEncryptionKey(0x2D3BB7CD, 0xA3895E7F)),
            new EncryptionVersion("3.0.2", new LoginEncryptionKey(0x2DE3ADDD, 0xA3E5227F)),
            new EncryptionVersion("3.0.1", new LoginEncryptionKey(0x2DABA7ED, 0xA3C17E7F)),
            new EncryptionVersion("3.0.0", new LoginEncryptionKey(0x2D93A5FD, 0xA3DD527F)),
            new EncryptionVersion("2.0.9", new LoginEncryptionKey(0x2CEB076D, 0xA363BE7F)),
            new EncryptionVersion("2.0.8", new LoginEncryptionKey(0x2CD3257D, 0xA37F527F)),
            new EncryptionVersion("2.0.7", new LoginEncryptionKey(0x2C9BC78D, 0xA35BFE7F)),
            new EncryptionVersion("2.0.6", new LoginEncryptionKey(0x2C43ED9D, 0xA334227F)),
            new EncryptionVersion("2.0.5", new LoginEncryptionKey(0x2C0B97AD, 0xA310DE7F)),
            new EncryptionVersion("2.0.4", new LoginEncryptionKey(0x2DF385BD, 0xA3ED127F)),
        };

        public string Name { get; }
        public LoginEncryptionKey Key { get; }

        public EncryptionVersion(string name, LoginEncryptionKey key)
        {
            Name = name;
            Key = key;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is EncryptionVersion version)
            {
                if (string.IsNullOrEmpty(version.Name))
                    return string.IsNullOrEmpty(Name);

                return version.Name.Equals(Name, StringComparison.Ordinal);
            }

            return false;
        }

        public override int GetHashCode() => Name.GetHashCode();
    }
}
