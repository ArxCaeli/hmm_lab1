using System;
using System.Collections;
using System.Collections.Generic;

namespace HmmFramework
{
	public class ForwardBackward
	{
		List<HMM> m_HmmList;
		List<InputLine> m_Sequence;
		int m_WorkingStatesQty;
		List<HMM> m_WorkingStates;	
		int m_FullSequenceCount;
		HMM m_BeginingState;
		HMM m_EndingState;

		double[,] BackwardValues;
		double[,] ForwardValues;

		public ForwardBackward (List<HMM> HmmList, List<InputLine> Sequence)
		{
			m_HmmList = HmmList;
			m_Sequence = Sequence;
			m_WorkingStates = new List<HMM>();
			m_WorkingStatesQty = 0;
			foreach (HMM Hmm in HmmList)
				if (!(Hmm.IsFirst || Hmm.IsFinal))
				{
					m_WorkingStatesQty++;
					m_WorkingStates.Add(Hmm);
				}

			m_BeginingState = HmmList.Find(x => x.IsFirst == true);
			m_EndingState = HmmList.Find(x => x.IsFinal == true);

			m_FullSequenceCount = Sequence.Count + 2; // + 2 = for ending and beg
			BackwardValues = new double[m_FullSequenceCount,m_WorkingStatesQty]; 
			ForwardValues = new double[m_FullSequenceCount,m_WorkingStatesQty];

			for(int I = 0; I < m_FullSequenceCount; I++)
				for(int J = 0; J < m_WorkingStatesQty; J++)
				{
					BackwardValues[I,J] = 0;
					ForwardValues[I,J] = 0;
				}
			// backward L + 1;
			for (int I = 0; I < m_WorkingStatesQty; I++)
				BackwardValues[m_FullSequenceCount - 1, I] = 1;
			// Forward 0
			for (int I = 0; I < m_WorkingStatesQty; I++)
				ForwardValues[0, I] = 1;
				
		}

		public void CalcBackward ()
		{
			FirstBackward();
			for (int I = m_Sequence.Count - 2; I >= 0; I--)
				Backward(I);
			BackwardEnding();
		}

		/// <summary>
		/// Last char in sequence.
		/// </summary>
		public void FirstBackward ()
		{
			int State = 0;
			foreach(HMM Hmm in m_WorkingStates)
			{
				BackwardValues[m_FullSequenceCount - 2,State] = Hmm.GetTransitionStrength(m_EndingState.Name);
				State++;
			}
		}

		public void Backward (int SeqPos)
		{
			int StateToCalc = 0;
			foreach (HMM Hmm in m_WorkingStates)
			{
				// SeqPos + 1 to shift for begining state
				BackwardValues[SeqPos + 1,StateToCalc] = CalcBackwardState(Hmm,SeqPos); 

				StateToCalc++;
			}
		}

		public void BackwardEnding ()
		{
			double EndingResult = CalcBackwardState(m_BeginingState,-1);
			for (int I = 0; I < m_WorkingStatesQty; I++)
				BackwardValues[0, I] = EndingResult;
		}

		private double CalcBackwardState(HMM Hmm, int SeqPos)
		{
			double Result = 0;
			int State = 0;
			double[] LogSums = new double[m_WorkingStatesQty];

			foreach (HMM HmmKp1 in m_WorkingStates)
			{
				LogSums[State] = Math.Log(Hmm.GetTransitionStrength(HmmKp1.Name)) +
					Math.Log(HmmKp1.GetEmmisionStrength(m_Sequence[SeqPos + 1].Sequence)) + 
					Math.Log(BackwardValues[SeqPos + 2,State]); // + 2 to shift from ending result 
				State++;
			}

			double MaxVal = LogSums[0];
			for (int I = 1; I < m_WorkingStatesQty; I++)
				MaxVal = Math.Max(MaxVal,LogSums[I]);
			for(int I = 0; I < m_WorkingStatesQty; I++)
				Result += Math.Exp(MaxVal + Math.Log(Math.Exp(LogSums[I] - MaxVal)));

			return Result;
		}

		#region Forward
		public void CalcForward ()
		{
			ForwardBegining();
			for (int I = 1; I < m_Sequence.Count; I++)
				Forward(I);
			ForwardEnding();
		}

		private void ForwardBegining ()
		{
			int State = 0;
			foreach (HMM Hmm in m_WorkingStates)
			{
				ForwardValues [1, State] = 
					Hmm.GetEmmisionStrength (m_Sequence[0].Sequence) *
					m_BeginingState.GetTransitionStrength (Hmm.Name);
				State++;			
			}		
		}

		private void ForwardEnding ()
		{
			double EndingResult = CalcForwardState(m_EndingState, m_FullSequenceCount - 2);
			for (int I = 0; I < m_WorkingStatesQty; I++)
				ForwardValues[m_FullSequenceCount - 1, I] = EndingResult;
		}

		private void Forward (int SeqPos)
		{
			// starts from 1; 0 - begining state
			int StateToCalc = 0;
			foreach (HMM Hmm in m_WorkingStates)
			{
				ForwardValues[SeqPos + 1,StateToCalc] = CalcForwardState(Hmm, SeqPos);
					
				StateToCalc++;					
			}
		}

		private double CalcForwardState (HMM Hmm, int SeqPos)
		{
			double Result = 0;
			int State = 0;
			double[] LogSums = new double[m_WorkingStatesQty];

			double EmmisionStr = 1; // for endingState
			if (SeqPos < m_Sequence.Count)
				EmmisionStr = Hmm.GetEmmisionStrength(m_Sequence[SeqPos].Sequence);

			foreach(HMM HmmKm1 in m_WorkingStates)
			{
				// explog trick 
				LogSums[State] = Math.Log(EmmisionStr) +
					Math.Log(HmmKm1.GetTransitionStrength(Hmm.Name)) +
					Math.Log(ForwardValues[SeqPos,State]); // seqpos - 1 + 1 to shift for begining state
				State++;
			}

			double MaxVal = LogSums[0];
			for(int I = 1; I < m_WorkingStatesQty; I++)
				MaxVal = Math.Max(MaxVal,LogSums[I]);

			for(int I = 0; I < m_WorkingStatesQty; I++)
				Result += Math.Exp(MaxVal + Math.Log(Math.Exp(LogSums[I] - MaxVal)));

			return Result;
		}
		#endregion

		private int GetWorkingStateIndex(string StateName)
		{
			int I = 0;
			foreach(HMM Hmm in m_WorkingStates)
				if (Hmm.Name == StateName)
					return I;
				else I++;
						

			return -1; //not found
		}

		public double GetBackwardValue(int Pos, int State)
		{
			return BackwardValues[Pos,State];
		}

		public double GetBackwardValue(int Pos, string StateName)
		{
			return BackwardValues[Pos,GetWorkingStateIndex(StateName)];
		}

		public double GetForwardValue(int Pos, int State)
		{
			return ForwardValues[Pos,State];
		}

		public double GetForwardValue(int Pos, string StateName)
		{
			return ForwardValues[Pos,GetWorkingStateIndex(StateName)];
		}
	}
}

