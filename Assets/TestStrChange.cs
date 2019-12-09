using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TestStrChange : MonoBehaviour
{
	public enum Type
	{
		minutes,
		second,
		hourse
	}

	public Type type = Type.hourse;
	public bool UseSpan = false;
	
	public static void FastChageStrByFloatValue(ref string input, int value)
        {
            if (input == null || input.Length <= 0) return;
            unsafe
            {
	            ReadOnlySpan<Char> readOnlySpan = input.AsSpan();
	            ref var refToFirstChar = ref Unsafe.AsRef<Char>(in readOnlySpan[0]);
	            var span = new Span<Char>(Unsafe.AsPointer(ref refToFirstChar), input.Length);
                
	            ItoaByStringRef(value, ref span);
	            
//	            span[0] = 'T';
//	            span[1] = 'e';
//	            span[2] = 'x';
//	            span[3] = 't';
//	            span[4] = ' ';
//	            span[5] = (char)value;
            }
        }

	private static void ItoaByStringRef(int n, ref Span<char> str)
	{
		//char[] result = new char[11]; // 11 = "-2147483648".Length
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
	}
	
	
	private string currentStr = new string(new char[4]);
	
	private Text TextComponent;
	// Use this for initialization
	void Start ()
	{
		Debug.Log(currentStr.Length);
		TextComponent = GetComponent<Text>();
		TextComponent.text = currentStr;
		
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
			switch (type)
			{
				case Type.minutes:
				{
					currentNumber = time.Minute;
					break;
				}
				case Type.second:
				{
					currentNumber = time.Second;
					break;
				}
				case Type.hourse:
				{
					currentNumber = time.Hour;
					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
			}
			
			if (UseSpan)
			{
				TextComponent.text = Empty;
				yield return null;
				FastChageStrByFloatValue(ref currentStr, currentNumber);
				TextComponent.text = currentStr;
				
				//TextComponent.cachedTextGenerator.Invalidate();
				//TextComponent.SetVerticesDirty();
			}
			else
			{
				TextComponent.text = currentNumber.ToString();
			}
			yield return new WaitForSeconds(1f);
		}
	}
	
}
