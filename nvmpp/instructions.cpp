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
}

void i_pop(vm* machine) {
	machine->memory->pop_null();
}


