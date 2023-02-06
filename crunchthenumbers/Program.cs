
using System.Globalization;
using System.Text.RegularExpressions;
using SimpleCalculator;

namespace crunch_the_numbers
{
    class ConsoleCalculatorApp
    {
        private const string INPUTFORMATERRORSTRING = "Input string was not in a correct format.";
        private const string ASSIGNMENTPATTERN = @"^([a-zA-Z]+)=(\-?\d+(?>\.\d+)?)$";
        private const string CALCULATIONSPATTERN = @"^((?>\[[a-zA-Z]+\]|\d+(?>\.\d+)?))(\-|\^|\+|\*|\/)((?>\[[a-zA-Z]+\]|\d+(?>\.\d+)?))$";
        private const string VARIABLEPATTERN = @"([a-zA-Z]+)";
        private const string S1 = ">";

        private ISimpleCalculator calc;
        private Dictionary<string, double?> ans = new Dictionary<string, double?>();
        public Dictionary<string, double?> Ans 
        { 
            get
            {
                return this.ans;
            } 
            private set
            {
                this.ans = value;
            } 
        }

        public ConsoleCalculatorApp(ISimpleCalculator calc){
            this.calc = calc;
        }

        private double? splitString(string input, out string message)
        {
            var line = input;
            double? result = null;
            message = string.Empty;

            var assMatch = Regex.Matches(line, ASSIGNMENTPATTERN);
            var calcMatch = Regex.Matches(line, CALCULATIONSPATTERN);

            result = processMatches(assMatch, calcMatch, out message);
            
            if (result != null)
            {
                this.Ans["ans"] = result;
            }

            return result;
        }

        private double? processMatches(MatchCollection assMatch, MatchCollection calcMatch, out string message)
        {
            double? result = null;
            message = string.Empty;
            
            try{
                // neither assignment nor calculation
                if (assMatch.Count == 0 && calcMatch.Count == 0)
                {
                    message = INPUTFORMATERRORSTRING;
                    return result;
                }

                // assignment
                if (assMatch.Count == 1)
                {
                    var variableName = assMatch.Select(x => x.Groups[1].Value).First();
                    var value = assMatch.Select(x => x.Groups[2].Value).First();

                    message = "Assigning value: " + value + " to variable: " + variableName;
                    this.Ans[variableName] = Double.Parse(value, NumberStyles.Float, CultureInfo.CreateSpecificCulture("en-US"));

                    return result;
                }

                // calculation
                if (calcMatch.Count == 1)
                {

                    var firstOperandSubString = calcMatch.Select(x => x.Groups[1].Value).First();
                    var operAtorSubString = calcMatch.Select(x => x.Groups[2].Value).First();
                    var secondOperandSubString = calcMatch.Select(x => x.Groups[3].Value).First();

                    double? firstOperand = 0.0;
                    double? secondOperand = 0.0;

                    firstOperand = checkoperandsForVariables(firstOperandSubString, out message);

                    if(!string.IsNullOrEmpty(message)){
                        return result;
                    }

                    secondOperand = checkoperandsForVariables(secondOperandSubString, out message);

                    if(!string.IsNullOrEmpty(message)){
                        return result;
                    }

                    result = calculate(new double[2] {(double)firstOperand, (double)secondOperand}, operAtorSubString);

                }
                return result;
            }catch(DivideByZeroException ex){
                message = "Tried to divide by zero";
                return result;
            }catch(Exception e){
                message = "An error occurred";
                return result;
            }
        }     

        private double? checkoperandsForVariables(string operandstring, out string message)
        {
            double? result = null;
            message = string.Empty;
            if(Regex.IsMatch(operandstring, VARIABLEPATTERN))
                {
                    operandstring = operandstring.Trim ('[', ']');
                    if (Ans.ContainsKey(operandstring))
                    {
                        Ans.TryGetValue(operandstring, out result);
                        if(result == null){
                            message = "Value could not be retrieved";
                        }
                    }
                    else
                    {
                        message = "Unused Variable used";
                    }
                } 
                else
                {
                    result = Double.Parse(operandstring, NumberStyles.Float, CultureInfo.CreateSpecificCulture("en-US"));
                }
            return result;
        }

        private double? calculate(double[] numbers, string operAtor)
        {
            double? result = null;
            if (numbers.Length == 2 && operAtor.Length == 1)
            {
                switch (operAtor)
                {
                    case "+":
                        result = calc.Calculate(numbers[0], numbers[1], SimpleCalculations.ADDITION);
                        break;
                    case "-":
                        result = calc.Calculate(numbers[0], numbers[1], SimpleCalculations.SUBTRACTION);
                        break;
                    case "*":
                        result = calc.Calculate(numbers[0], numbers[1], SimpleCalculations.MULTIPLICATION);
                        break;
                    case "/":
                        result = calc.Calculate(numbers[0], numbers[1], SimpleCalculations.DIVISION);
                        break;
                    case "^":
                        result = calc.Calculate(numbers[0], numbers[1], SimpleCalculations.EXPONENTIATION);
                        break;
                }
            }
            return result;
            
        }

        private static void Main(string[] args)
        {
            ConsoleCalculatorApp calc = new ConsoleCalculatorApp(new SimpleTwoOperandCalculator());

            double? tempAns = null;

            Console.WriteLine("Hello, this is the Crunch the numbers calculation service. Type /help for more infos");

            while (true)
            {

                Console.Write(S1);

                string? input = Console.ReadLine();

                if (input != null && input.Length > 0 && input[0] == '/')
                {
                    switch (input)
                    {
                        case "/help":
                            Console.WriteLine("You can enter simple calculations like '1+2' and let them calculate it by pressing enter.\r You can only use one operand at a time. If you want" +
                                " to use negative numbers you need to store them in a variable first. (More of that further down)\r The operands you can use are +, -, *, / and ^. Furthermore" +
                                ", you can assign a variable a value\r\n by using a syntax like 'hello=15'. \r This would result in a variable called 'hello' with a value of 15. \r To simply" +
                                " use the variable within a calculation use [] brackets: '[hello]*5'.\r The result of the last calculation will alwas be available unter the variable 'ans'.\r" +
                                "You can display the variables with the command '/list'. You can clear them by using '/clear'.\r To stop the Crunch the Numbers calculation service, use '/stop'." +
                                " To display this help text at any time use '/help'");
                            break;
                        case "/list":
                            if (calc.Ans.Count > 0)
                            {
                                foreach (KeyValuePair<string, double?> kvp in calc.Ans)
                                {
                                    Console.WriteLine("  [" + kvp.Key + "] : " + kvp.Value);
                                }
                            }
                            else
                            {
                                Console.WriteLine("No values stored yet");
                            }

                            break;
                        case "/clear":
                            calc.Ans.Clear();
                            Console.WriteLine("Variables Cleared");
                            break;
                        case "/stop":
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine(INPUTFORMATERRORSTRING);
                            break;
                    }
                }
                else if(input != null)
                {
                    var message = string.Empty;
                    tempAns = calc.splitString(input, out message);
                    if (!string.IsNullOrEmpty(message))
                    {
                        Console.WriteLine(message);
                    }
                    if (tempAns != null)
                    {
                        Console.WriteLine(" =" + tempAns);
                    }
                }

            }
        }



    }
    
    

}



