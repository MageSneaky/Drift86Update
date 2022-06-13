using System;

public interface IUserControl
{
	float GetHorizontalAxis { get; }

	float GetVerticalAxis { get; }

	bool ControlInUse { get; }
}
