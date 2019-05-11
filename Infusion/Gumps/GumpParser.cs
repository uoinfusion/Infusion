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

            while (position < gump.Commands.Length && gump.Commands[position] != '\0')
            {
                SkipWhiteSpace();
                if (position < gump.Commands.Length)
                {
                    Consume('{');
                    SkipWhiteSpace();
                    ParseControl();
                    SkipWhiteSpace();
                    Consume('}');
                }
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
                case "buttontileart":
                    ParseButtonTileArt();
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
                case "gumppic":
                    ParseGumpPic();
                    break;
                case "tilepichue":
                    ParseTilePicHue();
                    break;
                default:
                    ParseUnknown();
                    break;
            }
        }

        private void ParseTilePicHue()
        {
            unchecked
            {
                var x = ParseIntParameter();
                var y = ParseIntParameter();
                var itemId = (uint)ParseIntParameter();
                var hue = ParseIntParameter();

                if (parserProcessor is IProcessTilePicHue tilePicHueProcessor)
                    tilePicHueProcessor.OnTilePicHue(x, y, itemId, hue);
            }
        }

        private void ParseGumpPic()
        {
            var x = ParseIntParameter();
            var y = ParseIntParameter();
            var gumpId = ParseIntParameter();

            if (parserProcessor is IProcessGumpPic gumpPicProcessor)
                gumpPicProcessor.OnGumpPic(x, y, gumpId);
        }

        private void ParseCheckBox()
        {
            var x = ParseIntParameter();
            var y = ParseIntParameter();
            var uncheckId = ParseIntParameter();
            var checkId = ParseIntParameter();
            var initialState = ParseBoolParameter();
            var id = ParseIntParameter();

            if (parserProcessor is IProcessCheckBox checkBoxProcessor)
                checkBoxProcessor.OnCheckBox(x, y, new GumpControlId((uint)id), uncheckId, checkId, initialState);
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

            if (parserProcessor is IProcessTextEntry textEntryProcessor)
                textEntryProcessor.OnTextEntry(x, y, width, maxLength, text, new GumpControlId(id));
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

            if (parserProcessor is IProcessText textProcessor)
                textProcessor.OnText(x, y, hue, gump.TextLines[textId]);
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

            if (parserProcessor is IProcessButton buttonProcessor)
                buttonProcessor.OnButton(x, y, down, up, isTrigger, pageId, new GumpControlId(triggerId));
        }

        private void ParseButtonTileArt()
        {
            var x = ParseIntParameter();
            var y = ParseIntParameter();
            var width = ParseIntParameter();
            var height = ParseIntParameter();
            var isTrigger = ParseBoolParameter();
            var pageId = (uint)ParseIntParameter();
            var triggerId = (uint)ParseIntParameter();
            var gumpId = ParseIntParameter();
            ParseIntParameter(); // unknown1
            ParseIntParameter(); // unknown2
            ParseIntParameter(); // unknown3

            if (parserProcessor is IProcessButtonTileArt buttonProcessor)
                buttonProcessor.OnButtonTileArt(x, y, width, height, isTrigger, pageId, new GumpControlId(triggerId), gumpId);
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

            while (char.IsDigit(gump.Commands[position]) || gump.Commands[position] == '-')
            {
                position++;
            }

            var parameterString = gump.Commands.Substring(startPosition, position - startPosition);
            return int.Parse(parameterString, System.Globalization.NumberStyles.Integer);
        }

        private void SkipWhiteSpace()
        {
            while (position < gump.Commands.Length && char.IsWhiteSpace(gump.Commands[position]))
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
            if (gump.Commands[position] != c)
                throw new GumpException($"Expecting {c} at {position} but {gump.Commands[position]} found in: {gump.Commands}");
            position++;
        }
    }
}