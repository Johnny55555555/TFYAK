using System.Collections.Generic;

namespace ТФЯК__1
{
    public class SyntaxAnalyzer
    {
        private List<Token> tokens;
        private int pos;

        private List<SyntaxError> errors = new List<SyntaxError>();

        public List<SyntaxError> Analyze(List<Token> tokenList)
        {
            tokens = tokenList;
            pos = 0;

            while (pos < tokens.Count)
            {
                int startLine = Current()?.Line ?? -1;

                ParseOperator();

                SkipLine(startLine);
            }

            return errors;
        }

        private Token Current()
        {
            while (pos < tokens.Count &&
                   tokens[pos].Type == "разделитель")
            {
                pos++;
            }

            if (pos >= tokens.Count)
                return null;

            return tokens[pos];
        }

        private void Next()
        {
            pos++;
        }

        private void Error(string message)
        {
            var token = Current();

            if (token != null)
            {
                errors.Add(new SyntaxError
                {
                    Fragment = token.Lexeme,
                    Position = token.Position,
                    Description = message,
                    Line = token.Line,
                    Column = token.Start
                });
            }
            else if (pos > 0)
            {
                var last = tokens[pos - 1];

                errors.Add(new SyntaxError
                {
                    Fragment = "EOF",
                    Position = last.Position,
                    Description = message,
                    Line = last.Line,
                    Column = last.End
                });
            }
        }

        private void SkipLine(int line)
        {
            while (Current() != null && Current().Line == line)
                pos++;
        }

        // Метод Айронса
        private bool ParseOperator()
        {
            int save = pos;

            if (!ParseConst()) { pos = save; return false; }
            if (!ParseChar()) { pos = save; return false; }
            if (!ParseIdentifier()) { pos = save; return false; }
            if (!ParseLeftBracket()) { pos = save; return false; }
            if (!ParseNumber()) { pos = save; return false; }
            if (!ParseRightBracket()) { pos = save; return false; }
            if (!ParseAssign()) { pos = save; return false; }
            if (!ParseString()) { pos = save; return false; }
            if (!ParseSemicolon()) { pos = save; return false; }

            return true;
        }

        private bool ParseConst()
        {
            if (Current()?.Lexeme == "const")
            {
                Next();
                return true;
            }

            Error("Ожидался const");
            return false;
        }

        private bool ParseChar()
        {
            if (Current()?.Lexeme == "char")
            {
                Next();
                return true;
            }

            Error("Ожидался char");
            return false;
        }

        private bool ParseIdentifier()
        {
            if (Current()?.Type == "идентификатор")
            {
                Next();
                return true;
            }

            Error("Ожидался идентификатор");
            return false;
        }

        private bool ParseLeftBracket()
        {
            if (Current()?.Lexeme == "[")
            {
                Next();
                return true;
            }

            Error("Ожидался [");
            return false;
        }

        private bool ParseRightBracket()
        {
            if (Current()?.Lexeme == "]")
            {
                Next();
                return true;
            }

            Error("Ожидался ]");
            return false;
        }

        private bool ParseNumber()
        {
            if (Current()?.Type == "целое число")
            {
                Next();
                return true;
            }

            Error("Ожидалось число");
            return false;
        }

        private bool ParseAssign()
        {
            if (Current()?.Lexeme == "=")
            {
                Next();
                return true;
            }

            Error("Ожидался =");
            return false;
        }

        private bool ParseString()
        {
            if (Current()?.Type == "строка")
            {
                Next();
                return true;
            }

            Error("Ожидалась строка");
            return false;
        }

        private bool ParseSemicolon()
        {
            if (Current()?.Lexeme == ";")
            {
                Next();
                return true;
            }

            Error("Ожидался ;");
            return false;
        }
    }
}
