/*
The MIT License (MIT)

Copyright (c) 2017 Cary Miller

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

// Usage: In the inspector, punch in your desired settings and then 
// enter PLAY mode to apply.

using UnityEngine;
using UnityEngine.Assertions;

[ExecuteInEditMode]
public class DSPixelPerfectCamera : MonoBehaviour
{
	public float FinalUnitSize 		{ get { return finalUnitSize; } }
	public int   PixelsPerUnit 		{ get { return pixelsPerUnit; } }
	public int   VertUnitsOnScreen 	{ get { return verticalUnitsOnScreen; } }

	[SerializeField]
	private int pixelsPerUnit = 16;
	[SerializeField]
	private int verticalUnitsOnScreen = 4;
	private float finalUnitSize;
	private new Camera camera;

	void Awake()
	{
		camera = gameObject.GetComponent<Camera>();
		Assert.IsNotNull(camera);

		SetOrthographicSize();
	}

	void SetOrthographicSize()
	{
		ValidateUserInput();

		// get device's screen height and divide by the number of units 
		// that we want to fit on the screen vertically. this gets us
		// the basic size of a unit on the the current device's screen.
		var tempUnitSize = Screen.height / verticalUnitsOnScreen;

		// with a basic rough unit size in-hand, we now round it to the
		// nearest power of pixelsPerUnit (ex; 16px.) this will guarantee
		// our sprites are pixel perfect, as they can now be evenly divided
		// into our final device's screen height.
		finalUnitSize = GetNearestMultiple(tempUnitSize, pixelsPerUnit);

		// ultimately, we are using the standard pixel art formula for 
		// orthographic cameras, but approaching it from the view of:
		// how many standard Unity units do we want to fit on the screen?
		// formula: cameraSize = ScreenHeight / (DesiredSizeOfUnit * 2)
		camera.orthographicSize = Screen.height / (finalUnitSize * 2.0f);
	}

	int GetNearestMultiple(int value, int multiple)
	{
		int rem = value % multiple;
		int result = value - rem;
		if (rem > (multiple / 2))
			result += multiple;

		return result;
	}

	void ValidateUserInput()
	{
		if (pixelsPerUnit == 0)
		{
			pixelsPerUnit = 1;
			Debug.Log("Warning: Pixels-per-unit must be greater than zero. " +
			          "Resetting to minimum allowed.");
		}
		else if (verticalUnitsOnScreen == 0)
		{
			verticalUnitsOnScreen = 1;
			Debug.Log("Warning: Units-on-screen must be greater than zero." +
			          "Resetting to minimum allowed.");
		}
	}
}
