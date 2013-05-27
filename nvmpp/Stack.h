#include <deque>

template<class T>
class Stack
{
    private:
    std::deque<T> container;
    
    public:
    Stack()
    {
        
    }
    
    void Push(T obj)
    {
        container.push_back(obj);
    }
    
    T Pop()
    {
        T obj = container.back();
        container.pop_back();
        return obj;
    }
    
    int Size() {
        return container.size();
    }
};
