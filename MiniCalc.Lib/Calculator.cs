using System.Globalization;

namespace MiniCalc.Lib;

public static class Calculator {
    /// <summary>
    /// Math equation calculation function
    /// </summary>
    /// <param name="mathEquation">Math equation</param>
    /// <returns></returns>
    public static decimal? EvaluateMathExpression(string mathEquation) {
        var tokens = Tokenize(mathEquation);
        var rpn = ConvertTokensToRpn(tokens);

        var result = EvaluateRpnQueue(rpn);
        result = Math.Round(result, 7);
        return result;
    }

    /// <summary>
    /// Converts the input string into a list of tokens
    /// </summary>
    /// <param name="inputString">A math equation</param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    private static IEnumerable<Token> Tokenize(string inputString) {
        inputString = inputString.Trim();
        // Standardise 10,0 to 10.0
        inputString = inputString.Replace(",", ".");
        // Remove unnecessary whitespaces
        inputString = inputString.Replace(" ", "");

        var i = 0;
        while (i < inputString.Length) {
            var c = inputString[i];
            if (char.IsDigit(c) || c == '.') {
                var start = i;
                while (i < inputString.Length && char.IsDigit(inputString[i]) || c == '.') {
                    i++;
                }

                var numberStr = inputString[start..i];
                if (!decimal.TryParse(numberStr, NumberStyles.Number, CultureInfo.InvariantCulture, out var number))
                    throw new FormatException($"Invalid number format: {numberStr}");
                yield return new Token(TokenType.Number, number);
                continue;
            }

            switch (c) {
                case '+':
                    yield return new Token(TokenType.Add, 0);
                    i++;
                    continue;
                case '-':
                    yield return new Token(TokenType.Sub, 0);
                    i++;
                    continue;
                case '*':
                    yield return new Token(TokenType.Mul, 0);
                    i++;
                    continue;
                case '/':
                    yield return new Token(TokenType.Div, 0);
                    i++;
                    continue;
            }

            throw new FormatException($"Unexpected character: '{inputString[i]}'");
        }
    }

    /// <summary>
    /// Gets the precedence of the operator
    /// </summary>
    /// <param name="tokenType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static int Precedence(TokenType tokenType) {
        switch (tokenType) {
            case TokenType.Add:
            case TokenType.Sub:
                return 1;
            case TokenType.Mul:
            case TokenType.Div:
                return 2;
            case TokenType.Number:
            default:
                throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null);
        }
    }

    /// <summary>
    /// Determines if the operator is left associative
    /// (Required if operations such as sin, cos, etc. are added later)
    /// </summary>
    /// <param name="tokenType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static bool IsLeftAssociative(TokenType tokenType) {
        return tokenType switch {
            TokenType.Add or TokenType.Sub or TokenType.Mul or TokenType.Div => true,
            _ => throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null)
        };
    }

    /// <summary>
    /// Converts a list of tokens into Reverse Polish Notation
    /// </summary>
    /// <param name="tokens">Infix based list of tokens</param>
    /// <returns>Reverse Polish Notation queue of tokens</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static Queue<Token> ConvertTokensToRpn(IEnumerable<Token> tokens) {
        var output = new Queue<Token>();
        var operationStack = new Stack<Token>();

        foreach (var token in tokens) {
            var (tokenType, _) = token;

            switch (tokenType) {
                case TokenType.Number:
                    output.Enqueue(token);
                    continue;
                case TokenType.Add:
                case TokenType.Sub:
                case TokenType.Mul:
                case TokenType.Div: {
                    var currentPrecedence = Precedence(tokenType);

                    while (operationStack.Count > 0) {
                        var (topType, _) = operationStack.Peek();
                        if (topType == TokenType.Number) {
                            break;
                        }

                        var topPrecedence = Precedence(topType);

                        // Stop if the top operator has lower precedence
                        if (topPrecedence <= currentPrecedence &&
                            (topPrecedence != currentPrecedence || !IsLeftAssociative(tokenType))) break;

                        output.Enqueue(operationStack.Pop());
                    }

                    operationStack.Push(token);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        while (operationStack.Count > 0)
            output.Enqueue(operationStack.Pop());

        return output;
    }

    /// <summary>
    /// Evaluates the RPN token queue
    /// </summary>
    /// <param name="tokens">RPN ordered Queue</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static decimal EvaluateRpnQueue(Queue<Token> tokens) {
        var stack = new Stack<decimal>();
        while (tokens.Count > 0) {
            var (tokenType, potentialValue) = tokens.Dequeue();
            if (tokenType == TokenType.Number && potentialValue.HasValue) {
                stack.Push(potentialValue.Value);
            }
            else {
                // This would not be the case for sin, cos etc.
                if (stack.Count < 2)
                    throw new InvalidOperationException("Insufficient operands");

                var right = stack.Pop();
                var left = stack.Pop();

                var result = tokenType switch {
                    TokenType.Add => left + right,
                    TokenType.Sub => left - right,
                    TokenType.Mul => left * right,
                    TokenType.Div => left / right,
                    _ => throw new InvalidOperationException("Unsupported operation type")
                };
                stack.Push(result);
            }
        }

        if (stack.Count != 1) {
            throw new InvalidOperationException("Malformed expression.");
        }

        return stack.Pop();
    }
}
