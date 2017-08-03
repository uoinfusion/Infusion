using System;
using Infusion.Packets;

namespace Infusion.Gumps
{
    public sealed class GumpParser
    {
        private readonly IGumpParserProcessor parserProcessor;
        private Gump gump;

        private int position;

        public GumpParser(IGumpParserProcessor parserProcessor)
        {
            this.parserProcessor = parserProcessor;
        }

        public void Parse(Gump gump)
        {
            position = 0;
            this.gump = gump;

            while (position < gump.Commands.Length)
            {
                Consume('{');
                ParseControl();
                Consume('}');
            }
        }

        private void ParseControl()
        {
            var controlName = ParseName();
            switch (controlName.ToLower())
            {
                case "button":
                    ParseButton();
                    break;
                case "text":
                    ParseText();
                    break;
                case "checkbox":
                    ParseCheckBox();
                    break;
                case "textentry":
                    ParseTextEntry();
                    break;
                default:
                    ParseUnknown();
                    break;
            }
        }

        private void ParseCheckBox()
        {
            var x = ParseIntParameter();
            var y = ParseIntParameter();
            var uncheck = ParseIntParameter();
            var check = ParseIntParameter();
            var isChecked = ParseBoolParameter();
            var id = ParseIntParameter();

            parserProcessor.OnCheckBox(x, y, new GumpControlId((uint)id));
        }

        private void ParseTextEntry()
        {
            var x = ParseIntParameter();
            var y = ParseIntParameter();
            var width = ParseIntParameter();
            var maxLength = ParseIntParameter();
            var hue = ParseIntParameter();
            var id = (uint)ParseIntParameter();
            var text = ParseStringParameter();

            parserProcessor.OnTextEntry(x, y, width, maxLength, text, new GumpControlId(id));
        }

        private void ParseUnknown()
        {
            while (gump.Commands[position] != '}')
                position++;
        }

        private void ParseText()
        {
            var x = ParseIntParameter();
            var y = ParseIntParameter();
            uint hue = (uint)ParseIntParameter();
            var textId = ParseIntParameter();

            parserProcessor.OnText(x, y, hue, gump.TextLines[textId]);
        }

        private void ParseButton()
        {
            var x = ParseIntParameter();
            var y = ParseIntParameter();
            var down = ParseIntParameter();
            var up = ParseIntParameter();
            var isTrigger = ParseBoolParameter();
            var pageId = (uint)ParseIntParameter();
            var triggerId = (uint)ParseIntParameter();

            parserProcessor.OnButton(x, y, down, up, isTrigger, pageId, new GumpControlId(triggerId));
        }

        private bool ParseBoolParameter()
        {
            var parameter = ParseIntParameter();
            return Convert.ToBoolean(parameter);
        }

        private string ParseStringParameter()
        {
            SkipWhiteSpace();

            var startPosition = position;
            while (char.IsLetterOrDigit(gump.Commands[position]))
                position++;

            return gump.Commands.Substring(startPosition, position - startPosition);
        }

        private int ParseIntParameter()
        {
            SkipWhiteSpace();

            var startPosition = position;

            while (char.IsDigit(gump.Commands[position]))
            {
                position++;
            }

            var parameterString = gump.Commands.Substring(startPosition, position - startPosition);
            return int.Parse(parameterString);
        }

        private void SkipWhiteSpace()
        {
            while (char.IsWhiteSpace(gump.Commands[position]))
            {
                position++;
            }
        }

        private string ParseName()
        {
            var startNamePosition = position;

            do
            {
                position++;
            } while (char.IsLetter(gump.Commands[position]));

            return gump.Commands.Substring(startNamePosition, position - startNamePosition);
        }

        private void Consume(char c)
        {
            position++;
        }
    }
}