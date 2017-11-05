using System.Linq;

namespace Infusion
{
    public class DialogBox
    {
        public DialogBox(uint dialogId, ushort menuId, string question, DialogBoxResponse[] responses)
        {
            DialogId = dialogId;
            MenuId = menuId;
            Question = question;
            Responses = responses;
        }

        public uint DialogId { get; }
        public ushort MenuId { get; }
        public string Question { get; }
        public DialogBoxResponse[] Responses { get; }
        public string ResponseTexts => Responses.Select(r => r.Text).Aggregate(string.Empty, (l, r) => l + "; " + r);
    }
}