Label = "Repeat Left Mouse Click"
Description = "Press and release the left mouse button every 100ms. Stops if right mouse pressed."

function Run()
	Print("Down")
	MouseDown()
	Sleep(100)

	Print("Up")
	MouseUp()
	Sleep(100)
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