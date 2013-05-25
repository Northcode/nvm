using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncc
{
    public class Token
    {
        public TokenType type { get; set; }
        public object val { get; set; }
    }

    public enum TokenType
    {
        word,
        int_lit,
        float_lit,
        byte_lit,
        string_lit,
        bool_lit,
        symbol,
        newline
    }

    public class Scanner
    {
        public string code { set; get; }
        public List<Token> tokens { get; set; }

        public Scanner(string Code)
        {
            code = Code;
            tokens = new List<Token>();
        }

        public void Scan()
        {
            tokens = new List<Token>();
            int i = 0;
            while (i < code.Length)
            {
                if (char.IsDigit(code[i]))
                {
                    StringBuilder strb = new StringBuilder();
                    while (char.IsDigit(code[i]))
                    {
                        strb.Append(code[i]);
                        i++;
                    }
                    if (code[i] == '.')
                    {
                        strb.Append('.');
                        i++;
                        while (char.IsDigit(code[i]))
                        {
                            strb.Append(code[i]);
                            i++;
                        }
                        float f = (float)Convert.ToDecimal(strb.ToString());
                        tokens.Add(new Token() { type = TokenType.float_lit, val = f });
                    }
                    else if (code[i] == 'x')
                    {
                        i++;
                        strb.Append(code[i]);
                        i++;
                        strb.Append(code[i]);
                        i++;
                        byte b = byte.Parse(strb.ToString(), System.Globalization.NumberStyles.HexNumber);
                        tokens.Add(new Token() { type = TokenType.byte_lit, val = b });
                    }
                    else
                    {
                        string str = strb.ToString();
                        int n = Convert.ToInt32(str);
                        tokens.Add(new Token() { type = TokenType.int_lit, val = n });
                    }
                }
                else if (char.IsLetter(code[i]))
                {
                    StringBuilder strb = new StringBuilder();
                    while (i < code.Length && char.IsLetterOrDigit(code[i]))
                    {
                        strb.Append(code[i]);
                        i++;
                    }
                    if (strb.ToString() == "true")
                    {
                        tokens.Add(new Token() { type = TokenType.bool_lit, val = true });
                    }
                    else if (strb.ToString() == "false")
                    {
                        tokens.Add(new Token() { type = TokenType.bool_lit, val = false });
                    }
                    else
                    {
                        tokens.Add(new Token() { type = TokenType.word, val = strb.ToString() });
                    }
                }
                else if (code[i] == '"')
                {
                    bool escaped = false;
                    bool r = true;
                    StringBuilder strb = new StringBuilder();
                    i++;
                    r = (code[i] != '"');
                    while (r)
                    {
                        escaped = false;
                        if (code[i] == '\\')
                        {
                            escaped = true;
                        }
                        else
                        {
                            strb.Append(code[i]);
                        }
                        i++;
                        if (!escaped)
                        {
                            r = (code[i] != '"');
                        }
                    }
                    i++;
                    tokens.Add(new Token() { type = TokenType.string_lit, val = strb.ToString() });
                }
                else if (code[i] == '\n')
                {
                    tokens.Add(new Token() { type = TokenType.newline, val = "\n" });
                }
                else
                {
                    tokens.Add(new Token() { type = TokenType.symbol, val = code[i] });
                }
                i++;
            }
        }
    }
}
