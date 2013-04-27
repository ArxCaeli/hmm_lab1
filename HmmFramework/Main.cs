using System;

using System.Collections;
using System.Collections.Generic;

namespace HmmFramework
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args.Length != 0)
				InputManager.ReadFile (args [0]);

			if ((InputManager.HmmList.Count == 0) || (InputManager.Sequence.Count == 0))
				Console.WriteLine ("Empty input");

			InputManager.HmmList.Find (x => x.Name == "B").IsFirst = true;
			InputManager.HmmList.Find (x => x.Name == "E").IsFinal = true;

			ForwardBackward FB = new ForwardBackward (InputManager.HmmList, InputManager.Sequence);
			FB.CalcBackward ();
			FB.CalcForward ();

			ExportForwardBackwardResults.Export(FB,"result.txt");
		}
	}
}
