Label = "Repeat left mouse click"
Description = "Press and release the left mouse button every 100ms."

function Run()
	Print("Down")
	MouseDown()
	Sleep(100)

	Print("Up")
	MouseUp()
	Sleep(100)
end

function OnMouseDown()  
	Stop()
end

function Cleanup()
	Print("")
end