using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi
{
    public interface IUltimaClientWindow
    {
        void SetTitle(string title);
        WindowBounds? GetBounds();
        void Focus();

    }
}
