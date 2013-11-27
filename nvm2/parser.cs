using nvm2.compiler.ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nvm2.compiler
{
    class Parser
    {
        Token[] tokens;
        int i;

        public Token[] Tokens
        {
            set
            {
                tokens = value;
            }
        }

        public Token Current
        {
            get
            {
                return GetToken(i);
            }
        }

        public Token Next
        {
            get
            {
                return GetToken(i + 1);
            }
        }

        public bool EOT
        {
            get
            {
                return i < 0 || i > tokens.Length;
            }
        }

        public void Step()
        {
            i++;
        }

        public Token GetToken(int i)
        {
            if(!EOT)
            {
                return tokens[i];
            }
            else
            {
                throw new IndexOutOfRangeException("Index out of range: " + i);
            }
        }


        public stmt ParseStmt()
        {
            if(!EOT)
            {
                if(Current.type == TokenType.word)
                {
                    ParseWord();
                }
            }
        }

        private void ParseWord()
        {
            if(Next == TokenType.symbol)
            {
                if (Next == '=')
                {
                    
                }
            }
        }
    }
}
