#pragma once

#include <iostream>

using namespace std;

//DEFINE VARIABLE TYPE CODES
#define type_NULL 0x00
#define type_BYTE 0x01
#define type_INT 0x03
#define type_UINT 0x04
#define type_STRING 0x07

class block
{
public:
	unsigned int size;
	unsigned int address;
	bool free;
	block* next;
	block* prev;
	
	block() {}
	
	~block() {
		delete next;
		delete prev;
	}

	block* getNextFree(unsigned int Size) {
		if(size > Size && free) {
			return this;
		}
		else
		{
			if(next != nullptr) {
				return next->getNextFree(Size);
			}
			else
			{
				return nullptr;
			}
		}
	}

	block* findAddr(unsigned int ptr) {
		if(address == ptr) {
			return this;
		} else {
			return next->findAddr(ptr);
		}
	}

	void merge() {
		if(next->free) {
			size = size + next->size;
			block* tmp = next->next;
			delete next;
			next = tmp;
		} else {
			if(prev != nullptr) {
				prev->merge();
			}
		}
	}

	void dumpTree() {
		cout << "BLOCK: " << address << " , " << size << " is free: " << free << endl;
		if(next != nullptr) {
			next->dumpTree();
		}
	}
};

class callstack_unit
{
public:
	callstack_unit();
	~callstack_unit();

	unsigned int addr;
	unsigned int offset;
};

class ram
{
	char* data;
	unsigned int size;
	unsigned int pos;
	block* heap;
	//locals
	unsigned int localsPointer;
	//stack
	unsigned int stackStart;
	unsigned int stackPointer;
	unsigned int stackSize;
	//callstack
	unsigned int callStackStart;
	unsigned int callStackPointer;
	unsigned int callStackSize;
	//object-heap
	unsigned int heapPointer;

	unsigned int savedpos;

public:
	ram(unsigned int ProgramSize, unsigned int LocalsSize,unsigned int CallStackSize, unsigned int StackSize, unsigned int HeapSize) {
		//Load memory
		localsPointer = ProgramSize + 2;
		stackStart = localsPointer + LocalsSize * 4 + 2;
		stackSize = StackSize;
		stackPointer = stackStart + stackSize;
		callStackStart = stackPointer + 1;
		callStackSize = CallStackSize * 4;
		callStackPointer = callStackStart + callStackSize;
		heapPointer = callStackPointer + 2;
		size = heapPointer + HeapSize;

		//Allocate ram data
		data = new char[size];
		pos = 0;

		//Setup heap
		heap = new block();
		heap->address = heapPointer;
		heap->size = size - heapPointer;
		heap->free = true;
		heap->next = nullptr;
	}
	
	~ram() {
		delete [] data;
		delete heap;
	}

	//------------------- Position operations -------------------

	void setpos(unsigned int value) {
		pos = value;
	}

	unsigned int getpos() {
		return pos;
	}

	void savepos() {
		savedpos = pos;
	}

	void restorepos() {
		pos = savedpos;
	}

	//------------------- Memory Read/Write operations -------------------

	void write(char value) {
		if(pos < size) {
			data[pos] = value;
			//cout << "wrote: " << value << endl;
			pos++;
		}
	}

	void writeInt(int value) {
		char* charray = (char*)(&value);
		write(charray[0]);
		write(charray[1]);
		write(charray[2]);
		write(charray[3]);
	}
	
	void writeUInt(unsigned int u) {
		char* charray = (char*)(&u);
		write(charray[0]);
		write(charray[1]);
		write(charray[2]);
		write(charray[3]);
	}

	void writeString(const char* string) {
		//compute string length
		int l = 0;
		for(l = 0; string[l] != '\0'; l++);
		l++;
		
		for(int i = 0; i < l; i++) {
			write(string[i]);
		}
		write('\0');
	}

	char read() {
		char ch = data[pos];
		//cout << "read: " << ch << endl;
		pos++;
		return ch;
	}

	int readInt() {
		char ca = read();
		char cb = read();
		char cc = read();
		char cd = read();
		char charArray[] = { ca, cb, cc, cd };
		int* intptr = (int*)charArray;
		return *intptr;
	}
	
	unsigned int readUInt() {
		char ca = read();
		char cb = read();
		char cc = read();
		char cd = read();
		char charArray[] = { ca, cb, cc, cd };
		unsigned int* uintptr = (unsigned int*)charArray;
		return *uintptr;
	}

	char* readString() {
		unsigned int startpos = getpos();
		int charCount = 0;
		while(read() != '\0') {
			charCount++;
		}
		charCount++;
		
		setpos(startpos);
		char* string = new char[charCount];
		for(int i = 0; i < charCount; i++) {
			string[i] = read();
		}
		string[charCount - 1] = '\0';
		pos += charCount;
		return string;
	}

	void load_program(char* program) {
		for(int i = 0; program[i] != EOF; i++) {
			data[i] = program[i];
		}
	}

	void dmp_ram(bool csv,int linecout,bool cast) {
		for(int i = 0; i < size; i++) {
			if(cast) {
				cout << (unsigned int)((char)data[i]);
			} else {
				cout << data[i];
			}
			if(csv) {
				cout << ",";
			}
			if(i % linecout == 0) {
				cout << endl;
			}
		}
	}
	
	// ------------------- Memory allocation operations -------------------

	unsigned int MAlloc(unsigned int allocsize) {
		if(heap == nullptr) {
			heap = new block();
			heap->address = heapPointer;
			heap->size = size - heapPointer;
			heap->free = true;
			heap->next = nullptr;
		}

		block* region = heap->getNextFree(allocsize);

		if(region != nullptr) {
			block* newregion = new block();
			newregion->address = region->address + allocsize;
			newregion->size = region->size - allocsize;
			newregion->free = true;
			newregion->next = region->next;

			region->next = newregion;
			region->size = allocsize;
			region->free = false;
			return region->address;
		}
		else {
			cout << "Cannot allocate space for memory." << endl;
		}
	}

	unsigned int Alloc(char ch) {
		//Set size = size of data + 1 for data type storage
		const int allocsize = 2;

		//Allocate address
		unsigned int memoryAddress = MAlloc(allocsize);
		savepos();

		//Store data
		setpos(memoryAddress);
		write(type_BYTE);
		write(ch);
		restorepos();
	}

	unsigned int Alloc(int i) {
		//Set size = size of data + 1 for data type storage
		const int allocsize = 5;

		//Allocate address
		unsigned int memoryAddress = MAlloc(allocsize);

		//Store data
		savepos();
		setpos(memoryAddress);
		write(type_STRING);
		writeInt(i);
		restorepos();
	}

	unsigned int Alloc(char* s) {
		//Compute size of string
		int size = 0;
		for(size = 0; s[size] != '\0'; size++);

		//Set size = size of data + 1 for data type storage
		const int allocsize = size + 1;

		//Allocate address
		unsigned int memoryAddress = MAlloc(allocsize);

		//Store data
		savepos();
		setpos(memoryAddress);
		write(type_BYTE);
		writeString(s);
		restorepos();

		return memoryAddress;
	}

	void free(unsigned int addr) {
		block* region = heap->findAddr(addr);
		region->free = true;
		region->merge();
	}

	void dumpHeapBlocks() {
		heap->dumpTree();
	}


	//------------------- Stack operations -------------------

	void push(char ch) {
		if(stackPointer - 2 > stackStart) {
			stackPointer -= 2;
			savepos();
			setpos(stackPointer);
			write(type_BYTE);
			write(ch);
			restorepos();
		}
	}

	void push_int(int i) {
		if(stackPointer - 5 > stackStart) {
			stackPointer -= 5;
			savepos();
			setpos(stackPointer);
			write(type_INT);
			writeInt(i);
			restorepos();
		}
	}
	
	void push_uint(unsigned int i) {
		if(stackPointer - 5 > stackStart) {
			stackPointer -= 5;
			savepos();
			setpos(stackPointer);
			write(type_UINT);
			writeUInt(i);
			restorepos();
		}
	}

	void push_string(const char* string) {
		//compute string length
		int l = 0;
		for(l = 0; string[l] != '\0'; l++);
		l++;
		
		if(stackPointer - (l + 1) > stackStart) {
			stackPointer -= (l + 1);
			savepos();
			setpos(stackPointer);
			write(type_STRING);
			writeString(string);
			restorepos();
		}
	}

	char peek_type() {
		savepos();
		setpos(stackPointer);
		char ch = read();
		restorepos();
		return ch;
	}

	char pop() {
		savepos();
		setpos(stackPointer);
		char type = read();
		char value = read();
		stackPointer = getpos();
		restorepos();
		return value;
	}

	int pop_int() {
		savepos();
		setpos(stackPointer);
		char type = read();
		int value = readInt();
		stackPointer = getpos();
		restorepos();
		return value;
	}
	
	unsigned int pop_uint() {
		savepos();
		setpos(stackPointer);
		char type = read();
		unsigned int value = readUInt();
		stackPointer = getpos();
		restorepos();
		return value;
	}

	char* pop_string() {
		savepos();
		setpos(stackPointer);
		char type = read();
		char* value = readString();
		stackPointer = getpos();
		restorepos();
		return value;
	}

	void pop_null() {
		savepos();
		setpos(stackPointer);
		char type = read();
		if(type == type_BYTE) {
			read();
		} else if(type == type_INT) {
			readInt();
		} else if(type == type_STRING) {
			readString();
		}
		stackPointer = getpos();
		restorepos();
	}
	
	//------------------- Call Stack Stuff -------------------
	
	void push_callstack(unsigned int addr,unsigned int offset) {
		if(callStackPointer - 8 > callStackStart) {
			savepos();
			callStackPointer -= 8;
			setpos(callStackPointer);
			writeUInt(addr);
			writeUInt(offset);
			restorepos();
		}
	}
	
	unsigned int pop_callstack() {
		savepos();
		setpos(callStackPointer);
		unsigned int addr = readUInt();
		unsigned int offset = readUInt();
		callStackPointer = getpos();
		restorepos();
		return addr;
	}
};

/*
int main() {
	ram* r = new ram(16,4,128,512);
	const char* str = "hello world!\0";
	r->pushString(str);
	cout << r->popString() << endl;
	getchar();
	return 0;
}
*/
