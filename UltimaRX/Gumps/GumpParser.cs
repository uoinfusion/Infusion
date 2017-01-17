using System;

namespace UltimaRX.Gumps
{
    public class GumpParser
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
                default:
                    ParseUnknown();
                    break;
            }
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

            parserProcessor.OnButton(x, y, down, up, isTrigger, pageId, triggerId);
        }

        private bool ParseBoolParameter()
        {
            var parameter = ParseIntParameter();
            return Convert.ToBoolean(parameter);
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