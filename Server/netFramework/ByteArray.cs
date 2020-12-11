using System;

public class ByteArray
{
    const int DEFAULT_SIZE = 1024;

    private readonly int initialSize;
    private int capacity;

    /// <summary>
    /// buffer
    /// </summary>
    public byte[] bytes;

    //the index of read and write of the bytes
    public int readIndex;
    public int writeIndex;

    /// <summary>
    /// free space of the byte array
    /// </summary>
    public int Remain { get { return capacity - writeIndex; } }
    /// <summary>
    /// length of the data
    /// </summary>
    public int Length { get { return writeIndex - readIndex; } }

    public ByteArray(int size = DEFAULT_SIZE)
    {
        bytes = new byte[size];
        initialSize = size;
        capacity = size;
        readIndex = 0;
        writeIndex = 0;
    }

    public ByteArray(byte[] defaultBytes)
    {
        bytes = defaultBytes;
        initialSize = defaultBytes.Length;
        capacity = defaultBytes.Length;
        readIndex = 0;
        writeIndex = readIndex + defaultBytes.Length;
    }

    /// <summary>
    /// resize the buffer by the param size
    /// </summary>
    /// <param name="size"></param>
    public void Resize(int size)
    {
        if (size < Length) return;
        if (size < capacity) return;
        int n = 1;
        while (n < size) n *= 2;
        Array.Copy(bytes, readIndex, new byte[n], 0, Length);
        capacity = n;
        writeIndex = Length;
        readIndex = 0;
    }

    /// <summary>
    /// write bytes into buffer with offset and count
    /// </summary>
    /// <param name="bs"></param>
    /// <param name="offsite"></param>
    /// <param name="count"></param>
    /// <returns>
    /// the number of bytes that have been written into buffer
    /// </returns>
    public int Write(byte[] bs, int offset, int count)
    {
        if(Remain < count)
        {
            Resize(Length + count);
        }
        Array.Copy(bs, offset, bytes, writeIndex, count);
        writeIndex += count;
        return count;
    }

    /// <summary>
    /// read bytes from buffer with offset and count (used in bs)
    /// </summary>
    /// <param name="bs"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns>
    /// the number of bytes have been read
    /// </returns>
    public int Read(byte[] bs, int offset, int count)
    {
        count = Math.Min(count, Length);
        Array.Copy(bytes, readIndex, bs, offset, count);
        readIndex += count;
        CheckAndMove();
        return count;
    }

    /// <summary>
    /// check length and move bytes(data)
    /// </summary>
    public void CheckAndMove()
    {
        if(Length < 8)
        {
            Move();
        }
    }

    /// <summary>
    /// move bytes(data)
    /// </summary>
    public void Move()
    {
        Array.Copy(bytes, readIndex, bytes, 0, Length);
        writeIndex = Length;
        readIndex = 0;
    }

    /// <summary>
    /// get Int16(short) for counting
    /// </summary>
    /// <returns></returns>
    public Int16 ReadInt16()
    {
        if (Length < 2)
            return 0;
        Int16 res = BitConverter.ToInt16(bytes, readIndex);
        readIndex += 2;
        CheckAndMove();
        return res;
    }

    /// <summary>
    /// get Int32(int) for counting
    /// </summary>
    /// <returns></returns>
    public Int32 ReadInt32()
    {
        if (Length < 4)
            return 0;
        Int32 res = BitConverter.ToInt32(bytes, readIndex);
        readIndex += 4;
        CheckAndMove();
        return res;
    }

    public override string ToString()
    {
        return BitConverter.ToString(bytes, readIndex, Length);
    }

    public string Debug()
    {
        return string.Format("readIndex({0}), writeIndex({1}), allData({2})",
                      readIndex,
                      writeIndex,
                      BitConverter.ToString(bytes, 0, capacity));
    }
}
