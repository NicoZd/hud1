Label = "Press left mouse"
Description = "Press the left mouse button once."

function Setup()    
end

function Run()
	Print("Left mouse button is pressed until manually released or stopped.")
	MouseDown()
end

function Cleanup()
	MouseUp()
	Print("")
end