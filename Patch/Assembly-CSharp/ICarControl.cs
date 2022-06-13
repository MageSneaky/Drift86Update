using System;

public interface ICarControl
{
	float Horizontal { get; }

	float Vertical { get; }

	bool Brake { get; }
}
