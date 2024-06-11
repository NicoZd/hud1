Label = "Hold Left Mouse Button"
Description = "Press and hold the left mouse button. Stops if the right mouse button is pressed."

function Setup()    
	MouseDown(MouseButton.Left)
	Print("Left mouse pressed once.")
end

function OnMouseDown(button)  
	if button == MouseButton.Right then
		Stop()
	end
end

function Cleanup()
    MouseUp(MouseButton.Left)
    Print("Left mouse button released.")
	Wait(500)
	Print()
end