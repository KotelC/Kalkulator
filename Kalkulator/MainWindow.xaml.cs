using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kalkulator
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        List<char> operators;
        List<char> operands;
        double PI = Math.PI;

        public MainWindow()
        {
            InitializeComponent();
            operators = new List<char>() {
                            '²',
                            '+',
                            '-',
                            '*',
                            '%',
                            '/',
                            '✓',
                            
                          };
            operands = new List<char>() {
                            '1',
                            '2',
                            '3',
                            '4',
                            '5',
                            '6',
                            '7',
                            '8',
                            '9',
                            '0',
                            ',',
                            'π',
                            'e',
                          };
        }

        private void btnnumber_Click(object sender, RoutedEventArgs e)
        {
            var data = ((Button)sender).Content.ToString();
            ekran.Text += data;
        }

        private void OK_Button_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C)
            {
                ((Button)sender).Content = "WITAJ";
            }

            if (e.Key == Key.D)
            {
                ((Button)sender).Content = "ŻEGNAJ";
            }
        }

        private void equal_Click(object sender, RoutedEventArgs e)
        {
            if (CheckParentheses(ekran.Text))
            {
                var result = Calculate(GetReversePolishNotation(AddSquareToSquareFunction(ekran.Text)));
                if (result != null)
                    ekran.Text = result.ToString();
                else
                    clearbutton_Click(sender, e);
            }
            else
                MessageBox.Show("Źle nawiasy są tworzone", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private string AddSquareToSquareFunction(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '✓')
                {
                    if (i == 0)
                        input = "2" + input;
                    else if (!operands.Contains(input[i - 1]))
                        input = input.Insert(i, "2");
                }
            }
            return input;
        }

        public double? Calculate(List<string> expression)
        {
            if (expression == null || expression.Count == 0)
            {
                MessageBox.Show("Coś jest nie tak", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            
            var stack = new Stack<double>();

            
            foreach (var token in expression)
            {
                
                string num = "";
                switch (token)
                {
                    case "π":
                        num = Math.PI.ToString();
                        break;
                    case "e":
                        num = Math.E.ToString();
                        break;
                    default:
                        num = token;
                        break;
                }

               
                if (double.TryParse(num, out double number))
                    stack.Push(number);
                
                else if (operators.Contains(token[0]))
                {
                    
                    if (stack.Count < 2)
                    {
                        MessageBox.Show("Coś nie tak", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return null;
                    }

                    
                    double operand2 = stack.Pop();
                    double operand1 = stack.Pop();
                    try
                    {
                        
                        stack.Push(ApplyOperator(token, operand1, operand2));
                    }
                    catch (DivideByZeroException)
                    {
                        MessageBox.Show("Przestań dzielić przez zero nic to nie da", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return null;
                    }
                }
                else
                {
                    MessageBox.Show("Więcej niż jeden ( , ) nie nie da. Ponieważ jest to błąd w zapisie matematycznym", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }

            
            if (stack.Count != 1)
            {
                MessageBox.Show("Coś nie tak", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            
            return stack.Pop();
        }

        private double ApplyOperator(string token, double operand1, double operand2)
        {
            
            if (operand1 == 'π')
                operand1 = Math.PI;

            if (operand2 == 'π')
                operand2 = Math.PI;

            if (operand1 == 'e')
                operand1 = Math.E;

            if (operand2 == 'e')
                operand2 = Math.E;

            switch (token)
            {
                case "+":
                    return operand1 + operand2;
                case "-":
                    return operand1 - operand2;
                case "*":
                    return operand1 * operand2;
                case "²":
                    return Math.Pow(operand1, operand2);
                case "10²":
                    return Math.Pow(operand1, 10 * operand2);
                case "2²":
                    return Math.Pow(operand1, 2 * operand2);
                case "%":
                    return operand1 * (operand2 / 100);
                case "✓":
                    return Math.Round(Math.Pow(operand2, 1.0 / operand1), 2);
                case "/":
                    if (operand2 == 0)
                        throw new DivideByZeroException();
                    return operand1 / operand2;
                default:
                    throw new ArgumentException("Zły znak został wpisany: " + token); 
            }
        }

        private List<string> GetReversePolishNotation(string input)
        {
            Stack<char> stack = new Stack<char>();
            List<string> polishForm = new List<string>();
            string fullOperand = "";

            
            for (int i = 0; i < input.Length; i++)
            {
                
                if (operands.Contains(input[i]))
                {
                    
                    while (operands.Contains(input[i]))
                    {
                        fullOperand += input[i];
                        i++;
                        if (i == input.Length)
                        {
                            i--;
                            break;
                        }
                    }
                    
                    polishForm.Add(fullOperand);
                    fullOperand = "";
                }

                
                if (input[i] == '(')
                    stack.Push('(');

               
                if (input[i] == ')')
                {
                   
                    while (stack.First() != '(')
                        polishForm.Add(stack.Pop().ToString());
                    
                    stack.Pop();
                }

                
                if (operators.Contains(input[i]))
                {
                   
                    while (stack.Count != 0 && stack.First() != '(' && GetPrecedence(stack.First()) >= GetPrecedence(input[i]))
                        polishForm.Add(stack.Pop().ToString());
                   
                    stack.Push(input[i]);
                }
            }

         
            while (stack.Count != 0)
                polishForm.Add(stack.Pop().ToString());

            return polishForm;
        }

       
        private int GetPrecedence(char op)
        {
            switch (op)
            {
                case '✓': return 3;
                case '^': return 3;
                case '*': return 2;
                case '/': return 2;
                case '%': return 2;
                case '+': return 1;
                case '-': return 1;
                default:
                    return -1; 
            }
        }

        public static bool CheckParentheses(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return true; 

            var stack = new Stack<char>();

            foreach (char c in expression)
            {
                if (c == '(')
                    stack.Push(c);
                else if (c == ')')
                {
                    
                    if (stack.Count == 0)
                        return false;

                    
                    if (stack.Pop() != '(')
                        return false;
                }
            }

          
            return stack.Count == 0;
        }

        private void clearbutton_Click(object sender, RoutedEventArgs e)
        {
            ekran.Text = "";
        }

        private void delbutton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ekran.Text))
                ekran.Text = ekran.Text.Remove(ekran.Text.Length - 1);
        }


        private void Sin_Click(object sender, RoutedEventArgs e)
        {
            string t = ekran.Text.ToString();
            double p = Convert.ToDouble(t) * (PI/180) ;
            double p1 = Math.Sin(p);
            ekran.Text = p1.ToString();

        }

        private void Cos_Click(object sender, RoutedEventArgs e)
        {
            string t = ekran.Text.ToString();
            double p = Convert.ToDouble(t) * (PI / 180);
            double p1 = Math.Cos(p);
            ekran.Text = p1.ToString();

        }

        private void Tan_Click(object sender, RoutedEventArgs e)
        {
            string t = ekran.Text.ToString();
            double p = Convert.ToDouble(t) * (PI / 180);
            double p1 = Math.Tan(p);
            ekran.Text = p1.ToString();

        }

      
        


        private void change_Click(object sender, RoutedEventArgs e)
        {
            string math = ekran.Text.ToString();
            if (math[0] == '-')
            {
                math = math.Substring(1);
            }
            else
            {
                math = "-" + math;
            }
            ekran.Text = math;
        }

        private void etykieta1_KeyDown(object sender, KeyEventArgs e)
        {
            //if (ekran.Text)
        }

        private void Rand_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
