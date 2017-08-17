using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Blackjack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Deck deck = new Deck(6);
        Hand playerHand = new Hand();
        Hand playerSplitHand = new Hand();
        Hand dealerHand = new Hand();

        bool start = false;
        bool doub = false;
        bool split = false;

        double wager;
        double money = 500;

        public MainWindow()
        {
            InitializeComponent();

            lblMoney.Content = $"${money}";

            Setup();
        }

        public void Setup()
        {
            btnBet5.Opacity = 1;
            btnBet25.Opacity = 1;
            btnBet50.Opacity = 1;
            btnBet100.Opacity = 1;
            btnBetAll.Opacity = 1;
            btnBet5.IsEnabled = money >= 5;
            btnBet25.IsEnabled = money >= 25;
            btnBet50.IsEnabled = money >= 50;
            btnBet100.IsEnabled = money >= 100;
            btnBetAll.IsEnabled = true;
            btnNext.IsEnabled = false;
            btnNext.Opacity = 0;
            btnNext.SetValue(Panel.ZIndexProperty, 0);

            playerHand.Reset();
            playerSplitHand.Reset();
            dealerHand.Reset();
            lblPlayerHand.Opacity = 1;
            lblPlayerSplitHand.Opacity = 1;
            lblPlayerHand.Content = "";
            lblPlayerSplitHand.Content = "";
            lblDealerHand.Content = "";
            lblPoints.Content = "0";
            lblDealerPoints.Content = "0";
        }

        public void StartRound()
        {
            money -= wager;
            lblWager.Content = $"${wager}";
            lblMoney.Content = $"${money}";

            start = true;
            doub = false;

            btnBet5.Opacity = 0;
            btnBet25.Opacity = 0;
            btnBet50.Opacity = 0;
            btnBet100.Opacity = 0;
            btnBetAll.Opacity = 0;
            btnBet5.IsEnabled = false;
            btnBet25.IsEnabled = false;
            btnBet50.IsEnabled = false;
            btnBet100.IsEnabled = false;
            btnBetAll.IsEnabled = false;

            deck.Deal(dealerHand);
            deck.Deal(dealerHand);
            dealerHand.Cards[1].HideCard();
            deck.Deal(playerHand);
            deck.Deal(playerHand);
            lblPlayerHand.Content = playerHand.ToString();
            lblDealerHand.Content = dealerHand.ToString();
            lblPoints.Content = playerHand.GetPoints();
            lblDealerPoints.Content = dealerHand.GetPoints();
            
            btnStand.IsEnabled = true;
            btnSplit.IsEnabled = false;
            if (playerHand.GetPoints() == 21)
            {
                btnHit.IsEnabled = false;
                btnDouble.IsEnabled = false;
            }
            else
	        {
                btnHit.IsEnabled = true;
                btnDouble.IsEnabled = true;
                if (playerHand.Cards[0].GetPoints() == playerHand.Cards[1].GetPoints())
                {
                    btnSplit.IsEnabled = true;
                } 
            }
        }

        public void DisableSideRules()
        {
            if (start)
            {
                btnDouble.IsEnabled = false;
                btnSplit.IsEnabled = false;
                start = false;
            }
        }

        private void EndTurn()
        {
            if (!split)
            {
                btnHit.IsEnabled = false;
                btnStand.IsEnabled = false;
                btnDouble.IsEnabled = false;
                btnSplit.IsEnabled = false;

                dealerHand.Cards[1].ShowCard();
                lblDealerHand.Content = dealerHand.ToString();
                lblDealerPoints.Content = dealerHand.GetPoints();
                DealerTurn(); 
            }
            else
            {
                split = false;
                lblPlayerSplitHand.Opacity = 0.4;
                lblPlayerHand.Opacity = 1;
                lblPoints.Content = playerHand.GetPoints();
            }
        }
        

        private void DealerTurn()
        {
            if (dealerHand.GetPoints() > 16 || (playerHand.GetPoints() > 21  && (lblPlayerSplitHand.Content.ToString() == "" || playerSplitHand.GetPoints() > 21)))
            {
                EndRound();
            }
            else if (playerHand.GetPoints() == 21 && playerHand.Cards.Count == 2)
            {
                EndRound();
            }
            else
            {
                deck.Deal(dealerHand);
                lblDealerHand.Content = dealerHand.ToString();
                lblDealerPoints.Content = dealerHand.GetPoints();
                DealerTurn();
            }
        }

        void EvaluateRound(Hand playerHand, double wager)
        {
            double winnings = 0;

            if (playerHand.GetPoints() == 21 && playerHand.Cards.Count == 2)
            {
                if (dealerHand.GetPoints() == 21 && dealerHand.Cards.Count == 2)
                {
                    winnings = wager; 
                }
                else
                {
                    winnings = 2.5 * wager;
                }
            }
            else if (dealerHand.GetPoints() > 21 || (playerHand.GetPoints() > dealerHand.GetPoints() && playerHand.GetPoints() <= 21))
            {
                winnings = 2 * wager;
            }
            else if(playerHand.GetPoints() == dealerHand.GetPoints())
            {
                winnings = wager;
            }
            
            money += winnings;
        }

        private void EndRound()
        {
            if (doub)
            {
                wager *= 2;
            }

            if (lblPlayerSplitHand.Content.ToString() != "")
            {
                lblPlayerSplitHand.Opacity = 1;
                EvaluateRound(playerSplitHand, wager);
            }
            EvaluateRound(playerHand, wager);

            wager = 0;
            lblWager.Content = "$0";
            lblMoney.Content = $"${money}";

            if(money > 0)
            {
                btnNext.IsEnabled = true;
                btnNext.Opacity = 1;
                btnNext.SetValue(Panel.ZIndexProperty, 1);
            }
            else
            {
                lblGameOver.Opacity = 1;
            }
        }

        private void btnHit_Click(object sender, RoutedEventArgs e)
        {
            if (!split)
            {
                DisableSideRules();
                deck.Deal(playerHand);
                lblPlayerHand.Content = playerHand.ToString();
                lblPoints.Content = playerHand.GetPoints();
                if (playerHand.GetPoints() > 21)
                {
                    EndTurn();
                } 
            }
            else
            {
                deck.Deal(playerSplitHand);
                lblPlayerSplitHand.Content = playerSplitHand.ToString();
                lblPoints.Content = playerSplitHand.GetPoints();
                if (playerSplitHand.GetPoints() > 21)
                {
                    EndTurn();
                }
            }
        }

        private void btnStand_Click(object sender, RoutedEventArgs e)
        {
            DisableSideRules();
            EndTurn();
        }

        private void btnDouble_Click(object sender, RoutedEventArgs e)
        {
            DisableSideRules();
            doub = true;
            deck.Deal(playerHand);
            lblPlayerHand.Content = playerHand.ToString();
            lblPoints.Content = playerHand.GetPoints();
            EndTurn();
        }

        private void btnSplit_Click(object sender, RoutedEventArgs e)
        {
            DisableSideRules();
            split = true;
            playerHand.Deal(playerSplitHand);
            deck.Deal(playerSplitHand);
            deck.Deal(playerHand);

            lblPlayerSplitHand.Content = playerSplitHand.ToString();
            lblPlayerHand.Content = playerHand.ToString();
            lblPlayerHand.Opacity = 0.4;
            lblPoints.Content = playerSplitHand.GetPoints();

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Setup();
        }

        private void btnBet5_Click(object sender, RoutedEventArgs e)
        {
            wager = 5;
            StartRound();
        }

        private void btnBet25_Click(object sender, RoutedEventArgs e)
        {
            wager = 25;
            StartRound();
        }

        private void btnBet50_Click(object sender, RoutedEventArgs e)
        {
            wager = 50;
            StartRound();
        }

        private void btnBet100_Click(object sender, RoutedEventArgs e)
        {
            wager = 100;
            StartRound();
        }

        private void btnBetAll_Click(object sender, RoutedEventArgs e)
        {
            wager = money;
            StartRound();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            lblGameOver.Opacity = 0;
            money = 500;
            lblMoney.Content = $"${money}";
            start = false;
            doub = false;
            split = false;
            Setup();
        }
    }
}
