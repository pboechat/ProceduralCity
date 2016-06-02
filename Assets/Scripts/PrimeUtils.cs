using UnityEngine;
using System;
using System.Collections.Generic;

public class PrimeUtils
{
	public const int MAX_32_BITS_PRIME = 2147483647;
	
	private PrimeUtils ()
	{
	}
	
	public static List<int> GetPrimes (int start, int end)
	{
		List<int> primes = new List<int> ();
		for (int i = start; i <= end; i++) {
			if (IsPrime (i)) {
				primes.Add (i);
			}
		}
		return primes;
	}

	public static List<int> Decompose (int n)
	{
		return Decompose (n, 1, MAX_32_BITS_PRIME);
	}
	
	public static List<int> Decompose (int n, int start, int end)
	{
		List<int> primes = new List<int> ();
		while (n > 1) {
			for (int i = start; i <= end; i++) {
				if (IsPrime (i)) {                        
					if (n % i == 0) {
						n /= i;
						primes.Add (i);                            
						break;
					}
				}
			}
		}
		return primes;
	}
 
	public static bool IsPrime (int n)
	{
		if (n <= 1) {
			return false;
		}
		for (int i = 2; i <= Mathf.Sqrt(n); i++) {
			if (n % i == 0) {
				return false;
			}
		}
		return true;
	}
}