#include <iostream>
#include <deque>
#include "vm.h"

using namespace std;

int main()
{
    char prg[] = { PUSH, INT, 5, 0, 0, 0, END };
    int s = arraylen(prg);
    int* sp = &s;
    VM* v = new VM(prg,sp);
    v->Run();
    v->dmpstack();
    delete sp;
    delete v;
    return 0;
}