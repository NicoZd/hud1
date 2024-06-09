Label = "Sample Shortcut activated Macro"
Description = "Press and release the left mouse button every 100ms. Toggles on F5"

function Setup()
	active = false;	
end

function OnKeyDown(keyEvent)
	if (keyEvent.key == VK.F5) then
		active = not active;
	end
end

function Run()
	if (active) then
		Print("Down")
		MouseDown()
		Sleep(100)

		Print("Up")
		MouseUp()
		Sleep(100)
	else
		Print("Waiting for F5 to be pressed")
	end
end


function Cleanup()
	Print()
end