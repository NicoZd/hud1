Label = "Mouse Clicker"
Description = "Press and release the left mouse button every 100ms. Toggle with F5."

function Setup()
	active = false;	
end

function OnKeyDown(keyEvent)
	if (keyEvent.key == Key.F5) then
        active = not active
    end
end

function Run()
	if (active) then
		Print("Mouse Down")
		MouseDown(MouseButton.Left)
		Wait(100)

		Print("Mouse Up")
		MouseUp(MouseButton.Left)
		Wait(100)
	else
		Print("Press F5 to activate mouse clicker.")
	end
end

function Cleanup()
	Print()
end