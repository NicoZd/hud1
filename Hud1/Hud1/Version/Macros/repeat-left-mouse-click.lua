Label = "Repeat Left Mouse Click"
Description = "Press and release the left mouse button every 100ms. Toggle with F5."

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
		Print("Press F5 to activate mouse clicks.")
	end
end

function Cleanup()
	Print()
end