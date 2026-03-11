using System.Collections.Generic;
using System.Text;

namespace ТФЯК__1
{
    public class LexicalAnalyzer
    {
        public List<Token> Analyze(string text)
        {
            List<Token> tokens = new List<Token>();

            int i = 0;
            int line = 1;
            int column = 1;

            while (i < text.Length)
            {
                char c = text[i];

                if (c == ' ')
                {
                    int start = column;

                    while (i < text.Length && text[i] == ' ')
                    {
                        i++;
                        column++;
                    }

                    tokens.Add(new Token
                    {
                        Code = 4,
                        Type = "разделитель",
                        Lexeme = "пробел",
                        Position = $"{line} строка, с {start} по {column - 1}",
                        Line = line,
                        Start = start,
                        End = column - 1
                    });

                    continue;
                }

                if (c == '\t')
                {
                    tokens.Add(new Token
                    {
                        Code = 4,
                        Type = "разделитель",
                        Lexeme = "табуляция",
                        Position = $"{line} строка, с {column} по {column}",
                        Line = line,
                        Start = column,
                        End = column
                    });

                    i++;
                    column++;
                    continue;
                }

                if (c == '\r' || c == '\n')
                {
                    if (c == '\n')
                    {
                        line++;
                        column = 1;
                    }

                    i++;
                    continue;
                }

                if (char.IsLetter(c))
                {
                    int start = column;
                    StringBuilder sb = new StringBuilder();

                    while (i < text.Length && (char.IsLetterOrDigit(text[i]) || text[i] == '_'))
                    {
                        sb.Append(text[i]);
                        i++;
                        column++;
                    }

                    int end = column - 1;
                    string word = sb.ToString();

                    int code = 3;
                    string type = "идентификатор";

                    if (word == "const")
                    {
                        code = 1;
                        type = "ключевое слово";
                    }
                    else if (word == "char")
                    {
                        code = 2;
                        type = "ключевое слово";
                    }

                    tokens.Add(new Token
                    {
                        Code = code,
                        Type = type,
                        Lexeme = word,
                        Position = $"{line} строка, с {start} по {end}",
                        Line = line,
                        Start = start,
                        End = end
                    });

                    continue;
                }

                if (char.IsDigit(c))
                {
                    int start = column;
                    StringBuilder sb = new StringBuilder();

                    while (i < text.Length && char.IsDigit(text[i]))
                    {
                        sb.Append(text[i]);
                        i++;
                        column++;
                    }

                    int end = column - 1;

                    tokens.Add(new Token
                    {
                        Code = 9,
                        Type = "целое число",
                        Lexeme = sb.ToString(),
                        Position = $"{line} строка, с {start} по {end}",
                        Line = line,
                        Start = start,
                        End = end
                    });

                    continue;
                }

                if (c == '"')
                {
                    int start = column;
                    StringBuilder sb = new StringBuilder();

                    sb.Append(c);
                    i++;
                    column++;

                    bool closed = false;

                    while (i < text.Length)
                    {
                        if (text[i] == '"')
                        {
                            sb.Append('"');
                            i++;
                            column++;
                            closed = true;
                            break;
                        }

                        if (text[i] == '\n' || text[i] == '\r')
                        {
                            break;
                        }

                        sb.Append(text[i]);
                        i++;
                        column++;
                    }

                    int end = column - 1;

                    if (closed)
                    {
                        tokens.Add(new Token
                        {
                            Code = 8,
                            Type = "строка",
                            Lexeme = sb.ToString(),
                            Position = $"{line} строка, с {start} по {end}",
                            Line = line,
                            Start = start,
                            End = end
                        });
                    }
                    else
                    {
                        tokens.Add(new Token
                        {
                            Code = 0,
                            Type = "ERROR",
                            Lexeme = sb.ToString(),
                            Position = $"{line} строка, с {start} по {end}",
                            Line = line,
                            Start = start,
                            End = end
                        });
                    }

                    continue;
                }

                if (c == '[')
                {
                    tokens.Add(new Token
                    {
                        Code = 5,
                        Type = "открывающая скобка",
                        Lexeme = "[",
                        Position = $"{line} строка, с {column} по {column}",
                        Line = line,
                        Start = column,
                        End = column
                    });

                    i++;
                    column++;
                    continue;
                }

                if (c == ']')
                {
                    tokens.Add(new Token
                    {
                        Code = 6,
                        Type = "закрывающая скобка",
                        Lexeme = "]",
                        Position = $"{line} строка, с {column} по {column}",
                        Line = line,
                        Start = column,
                        End = column
                    });

                    i++;
                    column++;
                    continue;
                }

                if (c == '=')
                {
                    tokens.Add(new Token
                    {
                        Code = 7,
                        Type = "оператор присваивания",
                        Lexeme = "=",
                        Position = $"{line} строка, с {column} по {column}",
                        Line = line,
                        Start = column,
                        End = column
                    });

                    i++;
                    column++;
                    continue;
                }

                if (c == ';')
                {
                    tokens.Add(new Token
                    {
                        Code = 10,
                        Type = "конец оператора",
                        Lexeme = ";",
                        Position = $"{line} строка, с {column} по {column}",
                        Line = line,
                        Start = column,
                        End = column
                    });

                    i++;
                    column++;
                    continue;
                }

                tokens.Add(new Token
                {
                    Code = 0,
                    Type = "ERROR",
                    Lexeme = c.ToString(),
                    Position = $"{line} строка, с {column} по {column}",
                    Line = line,
                    Start = column,
                    End = column
                });

                i++;
                column++;
            }

            return tokens;
        }
    }
}
