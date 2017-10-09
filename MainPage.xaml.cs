using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace InkMath
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        InkInput inkInput;
        SpeechInput speechInput;
        MathProblem problem;

        MathProblem.Operators currentOp = MathProblem.Operators.Addition;

        string happyFace = "\u263A";
        string sadFace   = "\u2639";

        public MainPage()
        {
            this.InitializeComponent();
            this.inkInput = new InkInput(SolutionInkCanvas, CheckResult);
            this.speechInput = new SpeechInput();

            NewMathProblem();
        }

        private void NewMathProblem()
        {
            problem = new MathProblem(currentOp);
            EquationTextBlock.Text = problem.equation;
            ReadEquation();
        }

        private void NewMathProblem(MathProblem.Operators op)
        {
            currentOp = op;
            problem = new MathProblem(op);
            EquationTextBlock.Text = problem.equation;
            ReadEquation();
        }

        private async void SpeechButton_Click(object sender, RoutedEventArgs e)
        {
            string text = await this.speechInput.Recognize();
            
            RecoTextBlock.Text = text;
            CheckResult(text);
        }

        public string CommonMistakeFilter(string input)
        {
            if (input == "II")
                input = "11";

            if (input == "IS")
                input = "15";

            if (input == "O")
                input = "0";

            return input;
        }

        public void Right()
        {
            ResultTextBlock.Text = this.happyFace;
        }

        public void Wrong()
        {
            ResultTextBlock.Text = this.sadFace;
        }

        public void CheckResult(string result)
        {
            result = CommonMistakeFilter(result);

            if (problem.Solve(result))
            {
                Right();
                NewMathProblem(this.currentOp);
            }
            else
            {
                Wrong();
            }
        }

        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            ReadEquation();
        }

        private void ReadEquation()
        {
            SpeechReader.ReadText(problem.equation);
        }
    }
}
