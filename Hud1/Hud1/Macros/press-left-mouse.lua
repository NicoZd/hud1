Label = "Press left mouse"
Description = "Press the left mouse button once."

function Setup()    
	MouseDown()
end

function Run()
	if IsLeftMouseDown() then
		Running = false
	end 
end

function Cleanup()
	MouseUp()
end