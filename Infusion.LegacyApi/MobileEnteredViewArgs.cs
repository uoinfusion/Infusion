namespace Infusion.LegacyApi
{
    public struct MobileEnteredViewArgs
    {
        internal MobileEnteredViewArgs(Mobile newMobile)
        {
            NewMobile = newMobile;
        }

        public Mobile NewMobile { get; set; }
    }
}