using System;
using System.Text;
using System.IO;

public class Program
{
	static int Main(string[] argv)
	{
		Byte[] inputBytes = File.ReadAllBytes(argv[0]);
		String output = Encoding.Default.GetString(inputBytes);
		File.WriteAllText(argv[1], output);

		return 0;
	}
}