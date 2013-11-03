#include <iostream>
#include "ram.h"
#include "vm.h"

using namespace std;

int main() {
	ram* mem = new ram(32,512,512,512);
	char program[] = { 
		0x05, 0x02, 0x00, 0x00, 0x00, 0x03, 0x03, 0x05, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00,
		0x03, 0x07, 0x68, 0x65, 0x6C, 0x6C, 0x6F, 0x00, 0x08, 0x01, 0x00, 0x00, 0x00, 0x0A, 0x01, 0x00,
		0x00, 0x00, 0x02, 0x29, 0x00, 0x00, 0x00, 0x06, 0x1F, 0x05, 0x01, 0x00, 0x00, 0x00, 0x08, 0x00,
		0x00, 0x00, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x06,
		vmEOF 
	};
	mem->load_program(program);
	vm* v = new vm(mem);
	v->run();
	delete v;
	cout << "Program Terminated!" << endl;
	getchar();
	return 0;
}
