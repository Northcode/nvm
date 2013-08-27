#include <iostream>
#include "ram.h"
#include "vm.h"

using namespace std;

int main() {
	ram* mem = new ram(32,0,16,128,512);
	char program[] = { 0x05,0x00,0x00,0x00,0x00,0x02,0x0B,0x00,0x00,0x00,0x1F,0x05,0x00,0x00,0x00,0x00,0x1F,0x06,EOF };
	mem->load_program(program);
	vm* v = new vm(mem);
	v->run();
	mem->dmp_ram(true,10,true);
	delete v;
	getchar();
	return 0;
}
