using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    class Card
    {
        private int value;
        private char suit;
        private bool soft = false;
        private bool hidden = false;

        public int Value { get { return value; } private set { this.value = value; } }
        public char Suit { get { return suit; } private set { suit = value; } }
        public bool Soft { get { return soft; } set { soft = value; } }
        public bool Hidden { get { return hidden; } private set { hidden = value; } }

        public Card(int value, char suit)
        {
            Value = value;
            Suit = suit;
            if (value == 1)
            {
                Soft = true;
            }
        }

        public void HideCard()
        {
            Hidden = true;
        }
        public void ShowCard()
        {
            Hidden = false;
        }
        private char ValueToChar()
        {
            switch (Value)
            {
                case 10:
                    return 'A';
                case 11:
                    return 'B';
                case 12:
                    return 'D';
                case 13:
                    return 'E';
                default:
                    return char.Parse(Value + "");
            }
        }
        public override string ToString()
        {
            string unicode;
            if (Hidden)
            {
                unicode = "1F0A0";
            }
            else
            {
                unicode = $"1F0{Suit}{ValueToChar()}"; 
            }
            int code = int.Parse(unicode, System.Globalization.NumberStyles.HexNumber);
            string output = char.ConvertFromUtf32(code);
            return output;
        }

        public int GetPoints()
        {
            if (Hidden)
            {
                return 0;
            }
            switch (Value)
            {
                case 1:
                    if (Soft)
                    {
                        return 11;
                    }
                    else
                    {
                        return 1;
                    }
                case 11:
                    return 10;
                case 12:
                    return 10;
                case 13:
                    return 10;
                default:
                    return Value;
            }
        }
    }
}
