using System;

using System.IO;

namespace HmmFramework
{
	public static class ExportForwardBackwardResults
	{
		static public void Export(ForwardBackward FB, string FileName)
		{
			using(StreamWriter SW = new StreamWriter(FileName))
			{
				SW.WriteLine (FB.GetBackwardValue (0, 0) + "\t" + FB.GetBackwardValue (0, 1));
				SW.WriteLine (FB.GetForwardValue (InputManager.Sequence.Count + 1, 0) + "\t"
					+ FB.GetForwardValue (InputManager.Sequence.Count + 1, 1));

				double[] SeqPossibility = new double[InputManager.Sequence.Count]; 
				double[,] MaxMinPossibility = new double[InputManager.Sequence.Count,2];

				foreach (InputLine Seq in InputManager.Sequence)
				{
					SeqPossibility[Seq.SequenceNo] =
						FB.GetBackwardValue (Seq.SequenceNo + 1, Seq.StateName) * 
						FB.GetForwardValue (Seq.SequenceNo + 1, Seq.StateName) /
						FB.GetForwardValue (InputManager.Sequence.Count + 1, 0);

					MaxMinPossibility[Seq.SequenceNo,0] = 
						FB.GetBackwardValue (Seq.SequenceNo + 1, 0) * 
						FB.GetForwardValue (Seq.SequenceNo + 1, 0) /
						FB.GetForwardValue (InputManager.Sequence.Count + 1, 0);

					MaxMinPossibility[Seq.SequenceNo,1] = 
						FB.GetBackwardValue (Seq.SequenceNo + 1, 1) * 
						FB.GetForwardValue (Seq.SequenceNo + 1, 1) /
						FB.GetForwardValue (InputManager.Sequence.Count + 1, 0);

					if (MaxMinPossibility[Seq.SequenceNo,1] > MaxMinPossibility[Seq.SequenceNo,0])
					{
						double Tmp = MaxMinPossibility[Seq.SequenceNo,1];
						MaxMinPossibility[Seq.SequenceNo,1] = MaxMinPossibility[Seq.SequenceNo,0];
						MaxMinPossibility[Seq.SequenceNo,0] = Tmp;
					}
				}

				for(int I = 0; I < InputManager.Sequence.Count; I++)
					SW.WriteLine(MaxMinPossibility[I,1].ToString() + "\t" + 
					             SeqPossibility[I] + "\t" +
					             MaxMinPossibility[I,0] + "\t" +
					             InputManager.Sequence[I].Sequence + "\t" +
					             InputManager.Sequence[I].StateName);

			}

		}
	}
}

