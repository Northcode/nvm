#pragma once

#include "vm.h"
#include "opcodes.h"
#include "ram.h"

// ------------------ BRANCHING ------------------
void i_jmp(vm* machine) {
	unsigned int addr = machine->memory->readUInt();
	machine->IP = addr;
}

void i_end(vm* machine) {
	machine->RN = false;
}

void i_call(vm* machine) {
	unsigned int addr = machine->memory->readUInt();
	unsigned int retaddr = machine->memory->getpos();
	machine->memory->push_callstack(retaddr);
	machine->IP = addr;
}

void i_localheap(vm* machine) {
	unsigned int size = machine->memory->readUInt();
	machine->memory->set_callstack_size(size * 4);
	machine->IP = machine->memory->getpos();
}

void i_ret(vm* machine) {
	unsigned int addr = machine->memory->pop_callstack();
	machine->IP = addr;
}

// ------------------ STACK OPERATIONS ------------------

void i_push_byte(vm* machine) {
	char value = machine->memory->read();
	machine->memory->push(value);
}

void i_push_uint(vm* machine) {
	unsigned int value = machine->memory->readUInt();
	machine->memory->push_uint(value);
}

void i_push_int(vm* machine) {
	int value = machine->memory->readInt();
	machine->memory->push_int(value);
}

void i_push_string(vm* machine) {
	char* value = machine->memory->readString();
	machine->memory->push_string(value);
}

void i_push(vm* machine) {
	char type = machine->memory->read();
	switch(type) {
		case type_BYTE:
			i_push_byte(machine);
			break;
		case type_UINT:
			i_push_uint(machine);
			break;
		case type_INT:
			i_push_int(machine);
			break;
		case type_STRING:
			i_push_string(machine);
			break;
	}
	machine->IP = machine->memory->getpos();
}

void i_pop(vm* machine) {
	machine->memory->pop_null();
}

//---------------------- LOCALS -------------------------

void i_dmpstack(vm* machine) {
	cout << endl << "{[ERROR: DMPSTACK not impemented!]}" << endl;
}

void i_stloc(vm* machine) {
	unsigned int index = machine->memory->readUInt();
	char t = machine->memory->peek_type();
	if(t == type_BYTE) 
	{
		char v = machine->memory->pop();
		unsigned int addr = machine->memory->Alloc(v);
		machine->memory->stloc(index,addr);
	}
	else if(t == type_INT) 
	{
		int v = machine->memory->pop_int();
		unsigned int addr = machine->memory->Alloc(v);
		machine->memory->stloc(index,addr);
	}
	else if(t == type_STRING)
	{
		char* v = machine->memory->pop_string();
		unsigned int addr = machine->memory->Alloc(v);
		machine->memory->stloc(index,addr);
	}
	machine->IP = machine->memory->getpos();
}

void i_ldloc(vm* machine) {
	unsigned int index = machine->memory->readUInt();
	unsigned int addr = machine->memory->ldloc(index);
	unsigned int bpos = machine->memory->getpos();
	machine->memory->setpos(addr);
	char type = machine->memory->read();
	if(type == type_BYTE) {
		char val = machine->memory->read();
		machine->memory->push(val);
	}
	else if(type == type_INT) {
		int val = machine->memory->readInt();
		machine->memory->push_int(val);
	}
	else if(type == type_UINT) {
		unsigned int val = machine->memory->readUInt();
		machine->memory->push_uint(val);
	}
	else if (type == type_STRING) {
		char* val = machine->memory->readString();
		machine->memory->push_string(val);
	}
	machine->memory->setpos(bpos);
}


void i_freeloc(vm* machine) {
	unsigned int index = machine->memory->readUInt();
	machine->IP = machine->memory->getpos();
	machine->memory->freeloc(index);
}
