#include <iostream>

#define INT 1
#define CHAR 2
#define FLOAT 3
#define STRING 4

using namespace std;

class Object
{
    private:
    int type;
    char* obj;
    public:
    
    Object(int Type, char* Obj) {
        type = Type;
		obj = new char('\0');
        *obj = *Obj;
    }
    
    ~Object() {
        delete [] obj;
    }
    
    int GetType() {
        return type;
    }
    
    char* Char() {
        return obj;
    }
    
    int* Int() {
        return (int*)obj;
    }
    
    char** String() {
        return (char**)obj;
    }
    
    float* Float() {
        return (float*)obj;
    }
    
    void Output() {
        if(type == INT)
        {
            cout << *Int() << endl;
        }
        else if(type == CHAR)
        {
            cout << *Char() << endl;
        }
        else if(type == FLOAT)
        {
            cout << *Float() << endl;
        }
        else if(type == STRING)
        {
            cout << *String() << endl;
        }
    }
};
