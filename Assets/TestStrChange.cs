using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TestStrChange : MonoBehaviour
{
	public bool UseSpan = false;

	public static void FastChageStrByFloatValue(ref string input, int value)
        {
            if (input == null || input.Length <= 0) return;
            unsafe
            {
	            ReadOnlySpan<Char> readOnlySpan = input.AsSpan();
	            ref var refToFirstChar = ref Unsafe.AsRef<Char>(in readOnlySpan[0]);
	            var span = new Span<Char>(Unsafe.AsPointer(ref refToFirstChar), input.Length);
                
	            span[0] = 'T';
	            span[1] = 'e';
	            span[2] = 'x';
	            span[3] = 't';
	            span[4] = ' ';
	            span[5] = (char)value;
            }
        }

	
	private string currentStr = "Text  ";
	
	private Text TextComponent;
	// Use this for initialization
	void Start ()
	{
		TextComponent = GetComponent<Text>();
		TextComponent.text = currentStr;
		
		StartCoroutine(ChangeText());
	}

	private float time = 40f;

	private void OnDestroy()
	{
		StopAllCoroutines();
	}


	private const string Empty = "";
	private IEnumerator ChangeText()
	{
		while (true)
		{
			if (UseSpan)
			{
				TextComponent.text = Empty;
				yield return null;
				
				FastChageStrByFloatValue(ref currentStr, (int)time);
				TextComponent.text =  currentStr;
			}
			else
			{
				TextComponent.text = $@"{time:0.00}";
			}
			time += Time.deltaTime * 100;
			yield return new WaitForSeconds(1f);
		}
	}
	
}
