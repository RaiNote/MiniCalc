namespace MiniCalc.Lib;

public record struct Token(TokenType Type, decimal? Value = null);