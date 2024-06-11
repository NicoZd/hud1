Label = "Repeat keys WASD to walk in a diamond shape."
Description = "Repeat pressing and releasing WASD keys to move in a diamond shape. The macro stops if the right mouse button is pressed."

-- https://learn.microsoft.com/en-us/uwp/api/windows.system.virtualkey

waitTime = 500

function Run()

	Print("Left Back")
	KeyDown(Key.A)
	KeyDown(Key.S)
	Wait(waitTime)
	KeyUp(Key.A)
	KeyUp(Key.S)

	Print("Left Front")
	KeyDown(Key.A)
	KeyDown(Key.W)
	Wait(waitTime)
	KeyUp(Key.A)
	KeyUp(Key.W)

	Print("Right Front")
	KeyDown(Key.W)
	KeyDown(Key.D)
	Wait(waitTime)
	KeyUp(Key.W)
	KeyUp(Key.D)

	Print("Right Back")
	KeyDown(Key.D)
	KeyDown(Key.S)
	Wait(waitTime)
	KeyUp(Key.D)
	KeyUp(Key.S)

end

function OnMouseDown(button)  
	-- stop if right mouse down
	if button == MouseButton.Right then
		Stop()
	end
end

function Cleanup()
	KeyUp(Key.W)
	KeyUp(Key.A)
	KeyUp(Key.S)
	KeyUp(Key.D)
	Print()
end