using System.Collections;

public class CastUtil
{
    // s8
    public static char ParseChar(string input, char defaultValue = '\0')
    {
        try
        {
            return char.Parse(input);
        }
        catch (System.Exception)
        {

        }
        return defaultValue;
    }
    // u8
    public static byte ParseByte(string input, byte defaultValue = 0)
    {
        try
        {
            return byte.Parse(input);
        }
        catch (System.Exception)
        {

        }
        return defaultValue;
    }

    // s16
    public static short ParseShort(string input, short defaultValue = 0)
    {
        try
        {
            return short.Parse(input);
        }
        catch (System.Exception)
        {

        }
        return defaultValue;
    }
    // u16
    public static ushort ParseUshort(string input, ushort defaultValue = 0)
    {
        try
        {
            return ushort.Parse(input);
        }
        catch (System.Exception)
        {

        }
        return defaultValue;
    }

    // s32
    public static int ParseInt(string input, int defaultValue = 0)
    {
        try
        {
            return int.Parse(input);
        }
        catch (System.Exception)
        {

        }
        return defaultValue;
    }
    // u32
    public static uint ParseUint(string input, uint defaultValue = 0)
    {
        try
        {
            return uint.Parse(input);
        }
        catch (System.Exception)
        {

        }
        return defaultValue;
    }

    // s64
    public static long ParseLong(string input, long defaultValue = 0)
    {
        try
        {
            return long.Parse(input);
        }
        catch (System.Exception)
        {

        }
        return defaultValue;
    }
    // u64
    public static ulong ParseUlong(string input, ulong defaultValue = 0)
    {
        try
        {
            return ulong.Parse(input);
        }
        catch (System.Exception)
        {

        }
        return defaultValue;
    }

    // float
    public static float ParseFloat(string input, float defaultValue = 0)
    {
        try
        {
            return float.Parse(input);
        }
        catch (System.Exception)
        {

        }
        return defaultValue;
    }
    // double
    public static double ParseDouble(string input, double defaultValue = 0)
    {
        try
        {
            return double.Parse(input);
        }
        catch (System.Exception)
        {

        }
        return defaultValue;
    }
}
