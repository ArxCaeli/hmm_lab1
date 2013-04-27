using System;

namespace HmmFramework
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args.Length != 0)
				InputManager.ReadFile(args[0]);

			if ((InputManager.HmmList.Count == 0) || (InputManager.Sequence.Count == 0))
				Console.WriteLine("Empty input");

			InputManager.HmmList.Find(x => x.Name == "B").IsFirst = true;
			InputManager.HmmList.Find(x => x.Name == "E").IsFinal = true;

			//InputManager.Sequence.RemoveRange(1,InputManager.Sequence.Count - 2);

			ForwardBackward FB = new ForwardBackward(InputManager.HmmList, InputManager.Sequence);
			FB.CalcBackward();
			FB.CalcForward();

			//for(int I = 0; I < InputManager.Sequence.Count + 2; I++)
			//	Console.WriteLine(FB.GetBackwardValue(I,0) + "\t" + FB.GetBackwardValue(I,1));
			//for(int I = 0; I < InputManager.Sequence.Count + 2; I++)
			//	Console.WriteLine(FB.GetForwardValue(I,0) +
			//    	"\t" + FB.GetForwardValue(I,1));
			Console.WriteLine(FB.GetBackwardValue(0,0) + "\t" + FB.GetBackwardValue(0,1));
			Console.WriteLine(FB.GetForwardValue(InputManager.Sequence.Count + 1,0) + "\t"
			                  + FB.GetForwardValue(InputManager.Sequence.Count + 1,1));
			

		}
	}
}
