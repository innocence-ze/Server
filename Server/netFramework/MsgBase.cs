using System;
using System.Web.Script.Serialization;

public class MsgBase
{
    public string msgName;

    static JavaScriptSerializer Js = new JavaScriptSerializer();

    /// <summary>
    /// encode the data of msg
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static byte[] Encode(MsgBase msg)
    {
        string s = Js.Serialize(msg);
        return System.Text.Encoding.UTF8.GetBytes(s);
    }

    /// <summary>
    /// decode the data of msg
    /// </summary>
    /// <param name="bs"></param>
    /// <param name="offset">
    /// offset is the offset of data, rather the offset of msg
    /// </param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static MsgBase Decode(string name, byte[] bs, int offset, int count)
    {
        string s = System.Text.Encoding.UTF8.GetString(bs, offset, count);
        MsgBase msg = Js.Deserialize(s, Type.GetType(name)) as MsgBase;
        return msg;
    }

    /// <summary>
    /// encode the name of msg
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static byte[] EncodeName(MsgBase msg)
    {
        byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msg.msgName);
        Int16 nameLen = (Int16)nameBytes.Length;
        byte[] returnBytes = new byte[nameLen + 2];
        returnBytes[0] = (byte)(nameLen % 256);
        returnBytes[1] = (byte)(nameLen / 256);
        Array.Copy(nameBytes, 0, returnBytes, 2, nameLen);
        return returnBytes;
    }

    /// <summary>
    /// decode the name of msg
    /// </summary>
    /// <param name="bs"></param>
    /// <param name="offset">
    /// offset is the begining of nameByte rather the begining of the msg
    /// </param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static string DecodeName(byte[] bs, int offset, out int count)
    {
        count = 0;
        if (offset + 2 > bs.Length)
            return "";
        Int16 nameLen = (Int16)((bs[offset + 1] << 8) | bs[offset]);
        if (offset + 2 + nameLen > bs.Length)
            return "";

        count = 2 + nameLen;
        string name = System.Text.Encoding.UTF8.GetString(bs, offset + 2, nameLen);
        return name;
    }


}
