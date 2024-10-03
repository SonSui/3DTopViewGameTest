using System;

public class CircularBuffer<T>
{
    private T[] buffer;
    private int capacity;
    private int head;
    private int tail;
    private int size;

    public CircularBuffer(int capacity)
    {
        this.capacity = capacity;
        buffer = new T[capacity];
        head = 0;
        tail = 0;
        size = 0;
    }

    //push
    public void Add(T item)
    {
        buffer[head] = item;
        head = (head + 1) % capacity;

        if (size < capacity)
        {
            size++;
        }
        else
        {
            //Bufferはいっぱい、古いデータを捨てる
            tail = (tail + 1) % capacity;
        }
    }

    public T Get(int index)
    {
        if (index < 0 || index >= size)
            throw new IndexOutOfRangeException("Index out of range");

        int actualIndex = (tail + index) % capacity;
        return buffer[actualIndex];
    }

    public int Size
    {
        get { return size; }
    }

    // Poｐ
    public void RemoveLast()
    {
        if (size > 0)
        {
            size--;
            head = (head - 1 + capacity) % capacity;
        }
    }

    public void Clear()
    {
        head = 0;
        tail = 0;
        size = 0;
    }
}
