namespace SimpleCalculator;

class SimpleTwoOperandCalculator : ISimpleCalculator
{
    public double Calculate(double operand1, double operand2, SimpleCalculations calculationType)
    {
        switch(calculationType)
        {
            case SimpleCalculations.ADDITION:
                return Add(operand1, operand2);
            case SimpleCalculations.SUBTRACTION:
                return Subtract(operand1, operand2);
            case SimpleCalculations.MULTIPLICATION:
                return Multiply(operand1, operand2);
            case SimpleCalculations.DIVISION:
                return Divide(operand1, operand2);
            case SimpleCalculations.EXPONENTIATION:
                return Exponentiate(operand1, operand2);
        }

        throw new InvalidOperationException();
    }

    private static double Add(double operand1, double operand2)
    {
        return operand1 + operand2;
    }

    private static double Subtract(double operand1, double operand2)
    {
        return operand1 - operand2;
    }

    private static double Multiply(double operand1, double operand2)
    {
        return operand1 * operand2;
    }

    private static double Divide(double operand1, double operand2)
    {
        if(operand2 == 0)
        {
            throw new DivideByZeroException();
        }
        return operand1 / operand2;
    }

    private static double Exponentiate(double operand1, double operand2)
    {
        return Math.Pow(operand1, operand2);
    }
}

