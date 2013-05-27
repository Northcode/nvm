#include <iostream>
#include <sstream>
#include "Stack.h"
#include "Object.h"

//Arraylength function (DOES NOT WORK WITH POINTERS!)
#define arraylen(array) (sizeof((array))/sizeof((array[0])))

//OpCodes
#define NOP 0
#define JMP 1
#define CALL 2
#define END 3
#define RET 4
#define PUSH 5

using namespace std;

//Predefine so we can use it in classes
class VM;
class Buffer;

//Call class for callstacks
class Call {
    public:
    int raddr; //return address
    int inst; //instance id
    
    Call(int Raddr, int Inst) {
        raddr = Raddr;
        inst = Inst;
    }
    
    ~Call() {}
};

//Used to emulate RAM
class Buffer {
    private:
    char* data; //Byte array of data
    int size; //Size of emulated RAM
    
    public:
    Buffer(char* Data,int* datasize) {
        data = Data;
        size = *datasize;
    }
    
    Buffer(int* Size) {
        size = *Size;
        data = new char[*Size];
    }
    
    Buffer(int* Size, char* Data, int* Datasize) {
        size = *Size;
        data = new char[size];
        for(int i = 0; i < size;i++) {
            if(i < *Datasize) {
                data[i] = Data[i];
            }
            else {
                data[i] = '\0';
            }
        }
    }
    
    ~Buffer() {
        delete[] data; //Remember to delete pointers
    }
    
    char* Data() {
        return data;
    }
    
    int Size() {
        return size;
    }
    
    //Write byte
    void Write(int address, const char* val) {
        data[address] = *val;
    }
    
    //Write int
    void Write(int address, const int* val) {
        char* ch = (char*)val;
        data[address] = ch[address];
        data[address +1] = ch[address +1];
        data[address +2] = ch[address +2];
        data[address +3] = ch[address +3];
    }
    
    //Write string
    void Write(int address, char** val) {
        char* str = *val;
        int i = 0;
        for(i = 0; str[i] != '\0';i++) {
            data[address + i] = str[i];
        }
        data[address + i] = '\0';
        delete [] str; //Remove str
    }
    
    //Read byte
    char* Read(int address) {
        char ch = data[address];
        char* chp = &ch;
        return chp;
    }
    
    //Read int
    int* ReadInt(int address) {
        char ca = data[address];
        char cb = data[address + 1];
        char cc = data[address + 2];
        char cd = data[address + 3];
        char car[] = { ca, cb, cc, cd };
        int* ip = (int*)car; //Convert byte array to int
        return ip;
    }
    
    //Read string
    char** ReadString(int address, int* lenout) {
        int s = 0;
        for(s = 0; data[address +s] != '\0'; s++);
        char* ch = new char[s];
        for(int i = 0; i < s; i++) {
            ch[i] = data[address +i];
        }
        lenout = &s;
        char** chp = &ch;
        return chp;
    }
};

class VM {
    public:
    Stack<Object*> stack; //The stack
    Stack<Call> callstack; //The callstack (returnaddress, instanceid)
    unsigned int IP; //Instruction Pointer
    bool RN; //To run or not to run
    int progsize; //Program size
    Buffer* Memory; //RAM
    
    VM(char* program, int* psize) {
        IP = 0;
        int size = 512;
        progsize = *psize;
        Memory = new Buffer(&size,program,psize);
    }
    
    ~VM() {
        delete Memory;
    }
    
    void Run() {
        RN = true;
        while(RN & IP < progsize) {
            char* c = Memory->Read(IP);
            int i = (int)(*c);
			cout << IP << " OP:" << i << std::endl;
            IP++;
            
            if(i == JMP) {
                std::cout << "JMP" << std::endl;
                int* addr = Memory->ReadInt(IP);
                IP = *addr;
            }
            else if(i == CALL) {
                std::cout << "CALL" << std::endl;
                int* addr = Memory->ReadInt(IP);
                IP += 4;
                int cad = *addr;
                std::cout << "TO: " << *addr << ", FROM: " << IP << std::endl;
                Call c(IP,-1);
                callstack.Push(c);
                IP = cad;
            }
            else if(i == END) {
                std::cout << "END" << std::endl;
                RN = false;
            }
            else if(i == RET) {
                std::cout << "RET" << std::endl;
                Call c = callstack.Pop();
                IP = c.raddr;
            }
            else if(i == PUSH) {
                cout << "PUSH ";
                int type = (int)*(Memory->Read(IP));
                cout << "type: " << type << endl;
                IP++;
                if(type == INT) {
                    cout << "INT" << endl;
					int* val = new int();
                    *val = *Memory->ReadInt(IP);
                    IP += 4;
					Object* o = new Object(type,(char*)val);
                    stack.Push(o);
					delete val;
                } else if(type == CHAR) {
                    cout << "CHAR" << endl;
                    char* val = Memory->Read(IP);
                    IP++;
					Object* o = new Object(type,val);
                    stack.Push(o);
                } else if(type == FLOAT) {
                    cout << "ERROR: NOT IMPLEMENTED! PUSH FLOAT!" << endl;
                } else if(type == STRING) {
                    cout << "STRING" << endl;
                    int* p = 0;
                    char** val = Memory->ReadString(IP,p);
                    IP += *p;
					char* cp = (char*)val;
					Object* o = new Object(type,cp);
                    stack.Push(o);
                }
            }
        }
    }
    
    void dmpstack() {
        cout << "Dumping stack: size: " << stack.Size() << endl;
        while (stack.Size() > 0) {
            Object* o = stack.Pop();
            (*o).Output();
			delete o;
        }
    }
};