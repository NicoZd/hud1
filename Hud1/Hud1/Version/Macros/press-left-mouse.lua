Label = "Press Left Mouse"
Description = "Press the left mouse button once. Stops if right mouse pressed."

function Setup()    
	MouseDown()
	Print("Left mouse pressed once.")
end

function OnMouseDown(button)  
	-- stop if right mouse down
	if button == 2 then
		Stop()
	end
end

function Cleanup()
	MouseUp()
	Print()
end