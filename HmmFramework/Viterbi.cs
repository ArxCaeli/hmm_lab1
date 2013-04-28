using System;
using System.Collections;
using System.Collections.Generic;

namespace HmmFramework
{
	public struct ViterbiResult 
	{
		public int Position;
		public string StateName;
		public string PrevStateName;
		public double Possibility;

		public ViterbiResult (int NewPos, string NewStName, string PrevStName, double Possib)
		{
			Position = NewPos;
			StateName = NewStName;
			PrevStateName = PrevStName;
			Possibility = Possib;
		}
	}

	public class Viterbi
	{
		List<HMM> m_WorkingStates;
		List<InputLine> m_Sequence;
		ViterbiResult[,] Results;
		HMM m_BeginingState;
		HMM m_EndingState;

		public Viterbi (List<HMM> HmmList, List<InputLine> Sequence)
		{
			m_WorkingStates = new List<HMM>();
			m_Sequence = Sequence;
			foreach (HMM Hmm in HmmList)
				if (!(Hmm.IsFirst || Hmm.IsFinal))
					m_WorkingStates.Add(Hmm);
			m_BeginingState = HmmList.Find(x => x.IsFirst == true);
			m_EndingState = HmmList.Find(x => x.IsFinal == true);
			//+2 for fist and last state
			Results = new ViterbiResult[Sequence.Count + 2, m_WorkingStates.Count];
		}

		public void RunViterbi()
		{
			BeginningStep();

			foreach(InputLine Seq in m_Sequence)
				for(int I = 0; I < m_WorkingStates.Count; I++)
					MakeStep(Seq.SequenceNo + 1, m_WorkingStates[I], I);

			EndingStep();
		}

		private void MakeStep (int Pos, HMM State, int ArrayStateIndex)
		{
			double PrevMaxStateValue = 0;
			string PrevStateName = "";
			for (int I = 0; I < m_WorkingStates.Count; I++)
			{
				HMM PrevState = (Pos == 1) ? m_BeginingState : m_WorkingStates[I];
				double PrevStateValue = Results[Pos - 1, I].Possibility * 
					PrevState.GetTransitionStrength(State.Name);

				if (PrevStateValue > PrevMaxStateValue)
				{
					PrevMaxStateValue = PrevStateValue;
					PrevStateName = PrevState.Name;
				}
			}

			double Emission = 
				(Pos == m_Sequence.Count + 1) ? 
					1 : State.GetEmmisionStrength(m_Sequence[Pos - 1].Sequence);

			Results[Pos, ArrayStateIndex] = new ViterbiResult(
				Pos, State.Name, PrevStateName, Emission * PrevMaxStateValue);			
		}

		private void BeginningStep ()
		{
			for (int I = 0; I < m_WorkingStates.Count; I++)
				Results[0, I] = new ViterbiResult(
					0, m_BeginingState.Name, m_BeginingState.Name, 1);
		}

		private void EndingStep()
		{
			for(int I = 0; I < m_WorkingStates.Count; I++)
				MakeStep(m_Sequence.Count + 1, m_EndingState, I);
		}

		public ViterbiResult GetResult (int Position, HMM State)
		{
			int StateIndex = -1;
			for (int I = 0; I < m_WorkingStates.Count; I++)
				if (m_WorkingStates [I].Name == State.Name) 
				{
					StateIndex = I;
					break;
				}

			//cheat
			if (StateIndex == -1)
				StateIndex = 0;

			return Results[Position, StateIndex];			
		}
	}
}

