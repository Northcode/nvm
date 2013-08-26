#include <iostream>

#pragma once

using namespace std;

//DEFINE VARIABLE TYPE CODES
const char type_NULL = 0;
const char type_BYTE = 1;
const char type_INT = 3;
const char type_UINT = 4;
const char type_STRING = 7;

class block
{
public:
	unsigned int size;
	unsigned int address;
	bool free;
	block* next;
	block* prev;

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

class ram
{
	char* data;
	unsigned int size;
	unsigned int pos;
	block* heap;
	unsigned int localsPointer;
	unsigned int stackStart;
	unsigned int stackPointer;
	unsigned int stackSize;
	unsigned int heapPointer;

	unsigned int savedpos;
	
public:
	ram(unsigned int ProgramSize, unsigned int LocalsSize, unsigned int StackSize, unsigned int HeapSize) {
		//Load memory
		localsPointer = ProgramSize + 2;
		stackStart = localsPointer + LocalsSize * 4 + 2;
		stackSize = StackSize;
		stackPointer = stackStart + stackSize;
		heapPointer = stackPointer + 2;
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
	
	void writeString(const char* string) {
		while(string[pos] != '\0') {
			write(string[pos]);
		}
		write('\0');
	}
	
	char read() {
		char ch = data[pos];
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
			throw exception("Cannot allocate space for memory.");
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

	void pushInt(int i) {
		if(stackPointer - 5 > stackStart) {
			stackPointer -= 5;
			savepos();
			setpos(stackPointer);
			write(type_INT);
			writeInt(i);
			restorepos();
		}
	}

	void pushString(char* string) {
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

	char peekType() {
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

	int popInt() {
		savepos();
		setpos(stackPointer);
		char type = read();
		int value = readInt();
		stackPointer = getpos();
		restorepos();
		return value;
	}

	char* popString() {
		savepos();
		setpos(stackPointer);
		char type = read();
		char* value = readString();
		stackPointer = getpos();
		restorepos();
		return value;
	}

	void popNull() {
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
};