Label = "Repeat left mouse click"
Description = "Press and release the left mouse button every 100ms."

function Setup()  
end

function Run()
	Sleep(100)
	Print("Down")
	MouseDown()

	Sleep(100)
	Print("Up")
	MouseUp()

	if IsLeftMouseDown() then
		Running = false
	end 
end

function Cleanup()
	Print("")
end