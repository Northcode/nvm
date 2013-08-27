#include <iostream>
#include "ram.h"
#include "vm.h"

using namespace std;

int main() {
	ram* mem = new ram(32,4,16,128,1724);
	char program[] = { JMP, 10, 0, 0, 0, 0, 0, 0, 0, 0, END };
	vm* v = new vm(mem);
	v->memory->dmp_ram(false,50);
	delete v;
	getchar();
	return 0;
}
