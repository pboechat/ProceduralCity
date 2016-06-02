using System;

public interface IPatternItem : ICloneable
{
	void Initialize (char symbol, int index, ArchitectureStyle architectureStyle);
	
}