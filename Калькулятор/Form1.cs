using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ExpressionEvaluator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            string expression = txtExpression.Text;
            try
            {
                double result = EvaluateExpression(expression);
                lblResult.Text = $"Результат: {result}";
            }
            catch (Exception ex)
            {
                lblResult.Text = $"Помилка: {ex.Message}";
            }
        }

        private double EvaluateExpression(string expression)
        {
            expression = expression.Replace(" ", "");
            var tokens = Regex.Split(expression, @"([+\-*/])");
            var numbers = new Stack<double>();
            var operators = new Stack<char>();

            foreach (var token in tokens)
            {
                if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
                {
                    numbers.Push(number);
                }
                else if (IsOperator(token[0]))
                {
                    while (operators.Count > 0 && Precedence(operators.Peek()) >= Precedence(token[0]))
                    {
                        ProcessOperation(numbers, operators);
                    }
                    operators.Push(token[0]);
                }
            }

            while (operators.Count > 0)
            {
                ProcessOperation(numbers, operators);
            }

            return numbers.Pop();
        }

        private bool IsOperator(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/';
        }

        private int Precedence(char op)
        {
            switch (op)
            {
                case '+':
                case '-':
                    return 1;
                case '*':
                case '/':
                    return 2;
                default:
                    return 0;
            }
        }

        private void ProcessOperation(Stack<double> numbers, Stack<char> operators)
        {
            char op = operators.Pop();
            double right = numbers.Pop();
            double left = numbers.Pop();
            double result = ApplyOperation(left, right, op);
            numbers.Push(result);
        }

        private double ApplyOperation(double left, double right, char op)
        {
            switch (op)
            {
                case '+':
                    return left + right;
                case '-':
                    return left - right;
                case '*':
                    return left * right;
                case '/':
                    if (right == 0)
                    {
                        throw new DivideByZeroException("Ділення на нуль");
                    }
                    return left / right;
                default:
                    throw new InvalidOperationException("Невідома операція");
            }
        }
    }
}