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

			// forward-backward run
			ForwardBackward FB = new ForwardBackward (InputManager.HmmList, InputManager.Sequence);
			FB.CalcBackward ();
			FB.CalcForward ();
//
//			ExportForwardBackwardResults.Export(FB,args [0] + "_Result");

			// viterbi run
			Viterbi V = new Viterbi(InputManager.HmmList, InputManager.Sequence);
			V.RunViterbi();

			string[] MaxProbPath = GetMaxProbPath(V); 

			// results
			for (double Limit = 0.1; Limit < 0.99; Limit += 0.1)
				ShowStatistics(FB, MaxProbPath, Limit);

		}

		private static void ShowStatistics (ForwardBackward FB, string[] MaxProbPath, double Limit)
		{
			int TruePos = 0;
			int TrueNeg = 0;
			int FalsePos = 0;
			int FalseNeg = 0;
			string TrueRes = "St1";
			//string FalseRes = "St2";
			int Unresolved = 0;
			foreach (InputLine Seq in InputManager.Sequence)
			{
				double SeqPossibility =
					FB.GetBackwardValue (Seq.SequenceNo + 1, Seq.StateName) * 
					FB.GetForwardValue (Seq.SequenceNo + 1, Seq.StateName) /
					FB.GetForwardValue (InputManager.Sequence.Count + 1, 0);

				if (SeqPossibility > Limit)
				{
					if (MaxProbPath[Seq.SequenceNo + 1] == TrueRes)
					{
						if (Seq.StateName == TrueRes)
							TruePos++;
						else
							FalsePos++;
					}
					else
					{
						if (Seq.StateName == TrueRes)
							FalseNeg++;
						else
							TrueNeg++;
					}
				}
				else
					Unresolved++;
			}

			Console.WriteLine("======================================");
			Console.WriteLine("Limit: " + Limit.ToString());
			Console.WriteLine("TP: " + TruePos.ToString());
			Console.WriteLine("TN: " + TrueNeg.ToString());
			Console.WriteLine("FP: " + FalsePos.ToString());
			Console.WriteLine("FN: " + FalseNeg.ToString());
			Console.WriteLine("Unresolved: " + Unresolved.ToString());
		}

		private static string[] GetMaxProbPath (Viterbi V)
		{
			string[] MaxProbPath = new string[InputManager.Sequence.Count + 2];
			HMM Hmm = InputManager.HmmList.Find (x => x.Name == "E");
			for(int I = InputManager.Sequence.Count + 1; I >= 0; I--)
			{
				ViterbiResult VR = V.GetResult(I, Hmm);
						
				MaxProbPath[I] = VR.StateName;
				Hmm = InputManager.HmmList.Find(x => x.Name == VR.PrevStateName);
			}
			return MaxProbPath;
		}
	}
}
