using System.Text;

public static class ByteUtils
{
	public static string ToHex(this byte b)
	{
		return b.ToString("X2");
	}

	public static string ToHex(this byte[] bytes)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (byte b in bytes)
		{
			stringBuilder.Append(b.ToString("X2"));
		}
		return stringBuilder.ToString();
	}

	public static string ToHex(this byte[] bytes, string format)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (byte b in bytes)
		{
			stringBuilder.Append(b.ToString(format));
		}
		return stringBuilder.ToString();
	}

	public static string ToHex(this byte[] bytes, int offset, int count)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = offset; i < offset + count; ++i)
		{
			stringBuilder.Append(bytes[i].ToString("X2"));
		}
		return stringBuilder.ToString();
	}

	public static string ToStr(this byte[] bytes)
	{
		return Encoding.Default.GetString(bytes);
	}

	public static string ToStr(this byte[] bytes, int index, int count)
	{
		return Encoding.Default.GetString(bytes, index, count);
	}

	public static string Utf8ToStr(this byte[] bytes)
	{
		return Encoding.UTF8.GetString(bytes);
	}

	public static string Utf8ToStr(this byte[] bytes, int index, int count)
	{
		return Encoding.UTF8.GetString(bytes, index, count);
	}

	public static void WriteTo(this byte[] bytes, int offset, uint num)
	{
		bytes[offset] = (byte)(num & 0xff);
		bytes[offset + 1] = (byte)((num & 0xff00) >> 8);
		bytes[offset + 2] = (byte)((num & 0xff0000) >> 16);
		bytes[offset + 3] = (byte)((num & 0xff000000) >> 24);
	}

	public static void WriteTo(this byte[] bytes, int offset, int num)
	{
		bytes[offset] = (byte)(num & 0xff);
		bytes[offset + 1] = (byte)((num & 0xff00) >> 8);
		bytes[offset + 2] = (byte)((num & 0xff0000) >> 16);
		bytes[offset + 3] = (byte)((num & 0xff000000) >> 24);
	}
	public static int ReadTo(this byte[] bytes, int offset)
	{
		return (bytes[offset] & 255) | 
		       (bytes[offset + 1] & 255) << 8 | 
		       (bytes[offset + 2] & 255) << 16 |
		       (bytes[offset + 3] & 255) << 24;
	}

	public static long ReadToLong(this byte[] bytes, int offset)
	{
		return (bytes[offset] & 255) |
		       (bytes[offset + 1] & 255) << 8 |
		       (bytes[offset + 2] & 255) << 16 |
		       (bytes[offset + 3] & 255) << 24 |
		       (bytes[offset + 4] & 255) << 32 |
		       (bytes[offset + 5] & 255) << 40 |
		       (bytes[offset + 6] & 255) << 48 |
		       (bytes[offset + 7] & 255) << 56;
	}
	
	public static void WriteTo(this byte[] bytes, int offset, byte num)
	{
		bytes[offset] = num;
	}

	public static void WriteTo(this byte[] bytes, int offset, short num)
	{
		bytes[offset] = (byte)(num & 0xff);
		bytes[offset + 1] = (byte)((num & 0xff00) >> 8);
	}

	public static void WriteTo(this byte[] bytes, int offset, ushort num)
	{
		bytes[offset] = (byte)(num & 0xff);
		bytes[offset + 1] = (byte)((num & 0xff00) >> 8);
	}

	public static void WriteTo(this byte[] bytes, int offset, long num)
	{
		bytes[offset] = (byte)(num & 0xff);
		bytes[offset + 1] = (byte)((num & 0xff00) >> 8);
		bytes[offset + 2] = (byte)((num & 0xff0000) >> 16);
		bytes[offset + 3] = (byte)((num & 0xff000000) >> 24);
		bytes[offset + 4] = (byte)((num & 0xff00000000L) >> 32);
		bytes[offset + 5] = (byte)((num & 0xff0000000000L) >> 40);
		bytes[offset + 6] = (byte)((num & 0xff000000000000L) >> 48);
		bytes[offset + 7] = (byte)(unchecked((ulong)num & 0xFF00000000000000UL) >> 56);
	}
	
	public static void WriteToReverse(this byte[] bytes, int offset, int num)
	{
		bytes[offset + 3] = (byte)(num & 0xff);
		bytes[offset + 2] = (byte)((num & 0xff00) >> 8);
		bytes[offset + 1] = (byte)((num & 0xff0000) >> 16);
		bytes[offset] = (byte)((num & 0xff000000) >> 24);
	}
	
	public static void WriteToReverse(this byte[] bytes, int offset, long num)
	{
		bytes[offset + 7] = (byte)(num & 0xFF);
		bytes[offset + 6] = (byte)((num & 0xFF00) >> 8);
		bytes[offset + 5] = (byte)((num & 0xFF0000) >> 16);
		bytes[offset + 4] = (byte)((num & 0xFF000000L) >> 24);
		bytes[offset + 3] = (byte)((num & 0xFF00000000L) >> 32);
		bytes[offset + 2] = (byte)((num & 0xFF0000000000L) >> 40);
		bytes[offset + 1] = (byte)((num & 0xFF000000000000L) >> 48);
		bytes[offset]     = (byte)(unchecked((ulong)num & 0xFF00000000000000UL) >> 56);
	}
	
	public static int ReadToReverse(this byte[] bytes, int offset)
	{
		return (bytes[offset] & 255) << 24 | (bytes[offset + 1] & 255) << 16 | (bytes[offset + 2] & 255) << 8 | bytes[offset + 3] & 255;
	}
	
	public static long ReadToReverse(this byte[] bytes, long offset)
	{
		return ((long)(bytes[offset] & 255) << 56)     |
		       ((long)(bytes[offset + 1] & 255) << 48) |
		       ((long)(bytes[offset + 2] & 255) << 40) |
		       ((long)(bytes[offset + 3] & 255) << 32) |
		       ((long)(bytes[offset + 4] & 255) << 24) |
		       ((long)(bytes[offset + 5] & 255) << 16) |
		       ((long)(bytes[offset + 6] & 255) << 8)  |
		       (long)(bytes[offset + 7] & 255);
	}
}