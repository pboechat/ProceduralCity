using System;
using System.Linq;
using System.Collections.Generic;

public class SubsetSum
{
	private SubsetSum ()
	{
	}
	
	public static int[][] Combinations (int[] array, int k)
	{
		int[][][] ways = new int[k + 1][][];
		ways [0] = new[] { new int[0] };

		for (int i = 1; i <= k; i++) {
			ways [i] = (
            from val in array
            where i - val >= 0
            from subway in ways [i - val]
            where subway.Length == 0 || subway [0] >= val
            select Enumerable.Repeat (val, 1)
                .Concat (subway)
                .ToArray ()
        ).ToArray ();
		}

		return ways [k];
	}

}