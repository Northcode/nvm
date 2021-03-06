#pragma once

#include <iostream>
#include "ram.h"
#include "opcodes.h"

class vm
{
public:
	//Variables
	ram* memory;
	
	unsigned int IP;
	bool RN;

	//Initializers
	vm(ram* RAM) {
		memory = RAM;
		IP = 0;
		RN = false;
	}
	
	//Run methods
	void run();

};

#include "instructions.h"

void vm::run() {
	RN = true;
	while(RN) {
		memory->setpos(IP);
		char opcode = memory->read();
		cout << "current opcode: " << (int)opcode << endl;
		switch (opcode) {
			case JMP:
				i_jmp(this);
				break;
			case CALL:
				i_call(this);
				break;
			case PUSH:
				i_push(this);
				break;
			case LOCALHEAP:
				i_localheap(this);
				break;
			case END:
				i_end(this);
				break;
			case RET:
				i_ret(this);
				break;
			case STLOC:
				i_stloc(this);
				break;
			case LDLOC:
				i_ldloc(this);
				break;
			case FREELOC:
				i_freeloc(this);
				break;
		}
	}
}
