using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum StrType
{
	minutes,
	second,
	hourse
}

public class TestStrChange : MonoBehaviour
{
	

	public StrType strType;
	public bool UseSpan = false;

	private void FastClearStrWithoutNew(ref string str)
	{
		var span = GetStringSpan(ref str);

		for (int i = 0; i < span.Length; i++)
		{
			span[i] = ' ';
		}
	}

	private static Span<Char> GetStringSpan(ref string str)
	{
		unsafe
		{
			ReadOnlySpan<Char> readOnlySpan = str.AsSpan();
			ref var refToFirstChar = ref Unsafe.AsRef<Char>(in readOnlySpan[0]);
			var span = new Span<Char>(Unsafe.AsPointer(ref refToFirstChar), str.Length);
			return span;
		}
		
	}

	private static void FastChageStrByFloatValue(ref string input, int value)
        {
            if (input == null || input.Length <= 0) return;

            var span = GetStringSpan(ref input);
	        ItoaByStringRef(value, span);
        }

	private static void ItoaByStringRef(int n, Span<char> str)
	{
		int index = str.Length;
		bool sign = n < 0;

		do
		{
			int digit = n % 10;
			if(sign)
			{
				digit = -digit;
			}
			str[--index] = (char)('0' + digit);
			n /= 10;
		}
		while(n != 0);
		
		if(sign)
		{
			str[--index] = '-';
		}
		
		if (index != 0 || n == 0)
		{
			for (int i = 0; i < index; i++)
			{
				str[i] = '0';
			}
		}
	}
	
	
	private string currentStr;
	
	public Text TextComponent;
	
	// Use this for initialization
	void Start ()
	{
		currentStr = Guid.NewGuid().ToString().Substring(0, 2);
		FastClearStrWithoutNew(ref currentStr);
		TextComponent.text = currentStr;
		Debug.Log(currentStr.Length);
		StartCoroutine(ChangeText());
	}


	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	private string Empty = "";

	private DateTime time;


	private int currentNumber = -1;
	
	private IEnumerator ChangeText()
	{
		while (true)
		{
			time = DateTime.Now;
			switch (strType)
			{
				case StrType.minutes:
				{
					currentNumber = time.Minute;
					break;
				}
				case StrType.second:
				{
					currentNumber = time.Second;
					break;
				}
				case StrType.hourse:
				{
					currentNumber = time.Hour;
					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
			}
			
			if (UseSpan)
			{
				Profiler.BeginSample("Span change");
				FastChageStrByFloatValue(ref currentStr, currentNumber);
				TextComponent.cachedTextGenerator.Invalidate();
				TextComponent.SetVerticesDirty();
				Profiler.EndSample();
			}
			else
			{
				Profiler.BeginSample("Native change");
				TextComponent.text = currentNumber.ToString();
				Profiler.EndSample();
			}
			yield return new WaitForSeconds(1f);
		}
	}
	
}
