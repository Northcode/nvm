using System;
using System.Collections.Generic;
using System.Text;

namespace nvm2.compiler
{
    class Token
    {
        public object data;
        public TokenType type;

        public static bool operator==(Token token,TokenType type)
        {
            return token.type == type;
        }

        public static bool operator ==(Token token, int value)
        {
            if (token.data is int)
                return ((int)token.data) == value;
            else
                return false;
        }

        public static bool operator ==(Token token, string value)
        {
            if (token.data is string)
                return ((string)token.data) == value;
            else
                return false;
        }

        public static bool operator ==(Token token, float value)
        {
            if (token.data is float)
                return ((float)token.data) == value;
            else
                return false;
        }

        public static bool operator ==(Token token, bool value)
        {
            if (token.data is bool)
                return ((bool)token.data) == value;
            else
                return false;
        }

        public static bool operator ==(Token token, char value)
        {
            if (token.data is char)
                return ((char)token.data) == value;
            else
                return false;
        }

        public static bool operator !=(Token token, TokenType type)
        {
            return token.type != type;
        }

        public static bool operator !=(Token token, int value)
        {
            if (token.data is int)
                return ((int)token.data) != value;
            else
                return true;
        }

        public static bool operator !=(Token token, string value)
        {
            if (token.data is string)
                return ((string)token.data) != value;
            else
                return true;
        }

        public static bool operator !=(Token token, float value)
        {
            if (token.data is float)
                return ((float)token.data) != value;
            else
                return true;
        }

        public static bool operator !=(Token token, bool value)
        {
            if (token.data is bool)
                return ((bool)token.data) != value;
            else
                return true;
        }

        public static bool operator !=(Token token, char value)
        {
            if (token.data is char)
                return ((char)token.data) != value;
            else
                return true;
        }

        public static implicit operator int(Token token)
        {
            return (int)token.data;
        }

        public static implicit operator float(Token token)
        {
            return (float)token.data;
        }

        public static implicit operator string(Token token)
        {
            return token.data as string;
        }

        public static implicit operator bool(Token token)
        {
            return (bool)token.data;
        }

        public static implicit operator byte(Token token)
        {
            return (byte)token.data;
        }

        public override string ToString()
        {
            return "{ type: " + type.ToString() + " , value: " + data.ToString() + " }";
        }
    }

    enum TokenType
    {
        word,
        int_lit,
        float_lit,
        string_lit,
        bool_lit,
        byte_lit,
        symbol,
        newline
    }

    class Scanner
    {
        string code;
        List<Token> tokens;
        int i;

        public string Code
        {
            set
            {
                code = value;
            }
        }

        public List<Token> Tokens
        {
            get
            {
                return tokens;
            }
        }

        public Scanner()
        {
            tokens = new List<Token>();
            code = "";
            i = 0;
        }

        public char Current
        {
            get
            {
                return GetChar(i);
            }
        }

        public char Next
        {
            get
            {
                return GetChar(i + 1);
            }
        }

        public bool EOF
        {
            get
            {
                return !(i >= 0 && i < code.Length);
            }
        }

        void Step()
        {
            i++;
        }

        char GetChar(int i)
        {
            if (!EOF)
            {
                return code[i];
            }
            else
            {
                throw new Exception("Index out of range: " + i + " in code string");
            }
        }

        public void ScanAll()
        {
            i = 0;
            while (!EOF)
            {
                ScanNextToken();
            }
        }

        public void ScanNextToken()
        {
            if (!EOF && Current == '"')
            {
                ScanString();
            }
            else if (!EOF && char.IsLetter(Current))
            {
                ScanWord();
            }
            else if (!EOF && char.IsDigit(Current))
            {
                ScanNumber();
            }
            else if (!EOF && Current == '\n')
            {
                ScanNewLine();
            }
            else if (!EOF && char.IsWhiteSpace(Current))
            {
                Step(); //Skip Whitespace
            }
            else if (!EOF)
            {
                ScanSymbol(); //Any other character is a symbol
            }
        }

        void ScanWord()
        {
            StringBuilder stb = new StringBuilder();
            while (!EOF && char.IsLetterOrDigit(Current))
            {
                stb.Append(Current);
                Step();
            }

            string str = stb.ToString();

            if (str == "true" || str == "false")
            {
                tokens.Add(new Token() { type = TokenType.bool_lit, data = Convert.ToBoolean(str) });
            }
            else
            {
                tokens.Add(new Token() { type = TokenType.word, data = str });
            }
        }

        void ScanNumber()
        {
            StringBuilder stb = new StringBuilder();
            while (!EOF && char.IsDigit(Current))
            {
                stb.Append(Current);
                Step();
            }

            if (!EOF && Current == '.')
            {
                stb.Append('.');
                Step();
                while (!EOF && char.IsDigit(Current))
                {
                    stb.Append(Current);
                    Step();
                }
                float f = (float)Convert.ToSingle(stb.ToString());
                tokens.Add(new Token() { type = TokenType.float_lit, data = f });
            }
            else if (!EOF && Current == 'x')
            {
                stb.Clear();
                Step();
                while (!EOF && char.IsLetterOrDigit(Current))
                {
                    stb.Append(Current);
                    Step();
                }
                byte f = Convert.ToByte(stb.ToString(),16);
                tokens.Add(new Token() { type = TokenType.byte_lit, data = f });
            }
            else
            {
                int i = Convert.ToInt32(stb.ToString());
                tokens.Add(new Token() { type = TokenType.int_lit, data = i });
            }
        }

        void ScanString()
        {
            Step();
            StringBuilder stb = new StringBuilder();
            bool escaped = false;
            while (!EOF && (Current != '"' || escaped))
            {
                escaped = false;
                if (Current == '\\')
                {
                    escaped = true;
                }
                else
                {
                    stb.Append(Current);
                }
                Step();
            }
            Step(); //Skip last "
            tokens.Add(new Token() { type = TokenType.string_lit, data = stb.ToString() });
        }

        void ScanNewLine()
        {
            tokens.Add(new Token() { type = TokenType.newline, data = '\n' });
            Step();
        }

        void ScanSymbol()
        {
            tokens.Add(new Token() { type = TokenType.symbol, data = Current });
            Step();
        }
    }
}