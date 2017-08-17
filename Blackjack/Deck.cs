using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    class Deck
    {
        protected List<Card> cards;
        private int numberOfDecks;

        public List<Card> Cards { get { return cards; } set { cards = value; } }
        public int NumberOfDecks { get { return numberOfDecks; } private set { numberOfDecks = value; } }

        public Deck()
        {

        }
        public Deck(int numberOfDecks)
        {
            Cards = new List<Card>();
            NumberOfDecks = numberOfDecks;

            if(numberOfDecks > 0)
            {
                for (int i = 1; i < numberOfDecks; i++)
                {
                    for (int j = 1; j < 14; j++)
                    {
                        cards.Add(new Card(j, 'A'));
                        cards.Add(new Card(j, 'B'));
                        cards.Add(new Card(j, 'C'));
                        cards.Add(new Card(j, 'D'));
                    }
                }
            }
            
            Shuffle();
        }
        
        public override string ToString()
        {
            string output = "";
            foreach(Card c in Cards)
            {
                output += c.ToString();
            }
            return output;
        }

        public void Shuffle()
        {
            Random rnd = new Random();
            List<Card> shuffledCards = new List<Card>();
            int deckSize = cards.Count;

            for (int i = 0; i < deckSize; i++)
            {
                int random = rnd.Next(0, cards.Count - 1);
                shuffledCards.Add(cards[random]);
                cards.RemoveAt(random);
            }

            for (int i = 0; i < deckSize; i++)
            {
                int random = rnd.Next(0, shuffledCards.Count - 1);
                cards.Add(shuffledCards[random]);
                shuffledCards.RemoveAt(random);
            }
        }

        public void Reset()
        {
            cards.Clear();

            for (int i = 0; i < NumberOfDecks; i++)
            {
                for (int j = 1; j < 14; j++)
                {
                    cards.Add(new Card(j, 'A'));
                    cards.Add(new Card(j, 'B'));
                    cards.Add(new Card(j, 'C'));
                    cards.Add(new Card(j, 'D'));
                }
            }

            Shuffle();
        }

        public void Deal(Hand hand)
        {
            if(cards.Count > 0)
            {
                hand.cards.Add(cards[0]);
                cards.RemoveAt(0);
            }
            else
            {
                Reset();
                Deal(hand);
            }
        }
    }

    class Hand : Deck
    {
        public Hand()
        {
            cards = new List<Card>();
        }

        public new void Reset()
        {
            cards.Clear();
        }

        public int GetPoints()
        {
            //Declare point sum and sum the value of cards in hand
            int pointSum = 0;
            foreach (Card c in Cards)
            {
                pointSum += c.GetPoints();
            }

            //If point sum is 21 or less, no problems
            if(pointSum <= 21)
            {
                return pointSum;
            }

            //If point sum is greater than 21, search for "soft" aces and make them "hard"
            bool flag = true;
            foreach(Card c in Cards)
            {
                if (flag && c.Soft)
                {
                    c.Soft = false;
                    flag = false;
                }
            }

            //If no aces found, return point sum as is
            if (flag)
            {
                return pointSum;
            }

            //If ace found, run function again with hard ace
            flag = true;
            return GetPoints();
        }
    }
}
