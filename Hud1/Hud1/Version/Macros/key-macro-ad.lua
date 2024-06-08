Label = "Repeat pressing keys A & D"
Description = "Repeat pressing and releasing A and D. Stops if right mouse pressed."

-- https://learn.microsoft.com/en-us/uwp/api/windows.system.virtualkey

function Run()
	Print("Press A")
	KeyDown(VK.A)
	Sleep(100)

	Print("Release A")
	KeyUp(VK.A)

	Print("Press D")
	KeyDown(VK.D)
	Sleep(100)

	Print("Release D")
	KeyUp(VK.D)
end

function OnMouseDown(button)  
	-- stop if right mouse down
	if button == 2 then
		Stop()
	end
end

function Cleanup()
	Print()
end