while (true) {
    Console.WriteLine("If you wish to exit, use the common Ctrl + C combination");
    Console.WriteLine("Please enter your basic math equation:");
    Console.WriteLine();
    var input = Console.ReadLine();
    try {
        var calculationResult = MiniCalc.Lib.Calculator.EvaluateMathExpression(input);

        if (calculationResult == null) {
            Console.WriteLine("Invalid math equation.");
            continue;
        }

        Console.WriteLine(calculationResult);
    }
    catch (Exception e) {
        Console.WriteLine("An error occurred while calculating the equation.");
        Console.WriteLine($"Error: {e.Message}");
    }

    Console.WriteLine();
}
