using AdventOfCode.Solutions;
using AdventOfCode.Support;

// YearBuilder.CreateEmptyInputs();
await YearBuilder.CreateEmptySolutions();

var day = typeof(Day07);

Wrapper.Test(day);

Wrapper.Run(day, part: 1);
Wrapper.Run(day, part: 2);